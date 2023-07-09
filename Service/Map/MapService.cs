using Game_Realtime.Model.InGame;
using Game_Realtime.Model.Map;

namespace Game_Realtime.Service;

public class MapService
{
    private LogicTile[][] logicMap;
    private readonly int _width;
    private readonly int _height;
    public LogicTile[][] LogicMap
    {
        get => logicMap;
        private set => LogicMap = value;
    }
    public int Width { get { return _width; } }
    public int Height { get { return _height; } }
    public MapService(int height, int width,string playerId,string rivalPlayerId)
    {
        _width = width;
        _height = height;
        logicMap = new CreateLogicMapService(height, width).CreateLogicMap(playerId,rivalPlayerId).Result;
    }
    public bool IsValidPosition(Vector2Int logicPos, string playerId)
    {
        if (logicPos.y >= 0 && logicPos.y < _width)
        {
            if (logicMap[logicPos.x][logicPos.y].TypeOfType == TypeTile.Normal
                && logicMap[logicPos.x][logicPos.y].OwnerId == playerId)
            {
                // Console.WriteLine("Valid Position");
                return true;
            }
        }
        return false;
    }

    public List<Vector2Int> InitLongestPath()
    {
        Dictionary<TypePlayer, Vector2Int> _castleLogicPosition = new Dictionary<TypePlayer, Vector2Int> {
                { TypePlayer.Opponent, new Vector2Int(0, (_height-1)/2) },
                { TypePlayer.Player, new Vector2Int(_width-1, (_height-1)/2) }
            };
        int _columnIndexSplit = (_height - 1) / 2;
        Vector2Int _monsterGatePosition = new Vector2Int((_width - 1) / 2, _columnIndexSplit);

        Vector2Int startPos = new Vector2Int(_monsterGatePosition.x + 1, _monsterGatePosition.y);
        Vector2Int des = _castleLogicPosition[TypePlayer.Player];
        MapGraph graph = new MapGraph(_height, _width);
        for (int i = 0; i < logicMap.Length; i++)
        {
            for (int j = (_width - 1) / 2 + 1; j < logicMap[i].Length - 1; j++)
            {
                if (logicMap[i][j].TypeOfType == TypeTile.Normal)
                {
                    if (i < logicMap.Length - 1 && logicMap[i + 1][j].TypeOfType == TypeTile.Normal)
                    {
                        graph.AddEdge(new Vector2Int(j, i), new Vector2Int(j, i + 1));
                    }
                    if (j < logicMap[i].Length - 1 && logicMap[i + 1][j].TypeOfType == TypeTile.Normal)
                    {
                        graph.AddEdge(new Vector2Int(j, i), new Vector2Int(j + 1, i));
                    }
                }
            }
        }
        return graph.DFS(startPos, des, false);
    }
}