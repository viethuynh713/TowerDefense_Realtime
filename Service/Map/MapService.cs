using Game_Realtime.Model.InGame;
using Game_Realtime.Model.Map;
using Newtonsoft.Json;

namespace Game_Realtime.Service;

public class MapService
{
    private readonly LogicTile[][] _logicMap;
    private readonly int _width;
    private readonly int _height;
    private readonly int _columnIndexSplit;
    private readonly Vector2Int _monsterGatePosition;
    private Dictionary<string, Vector2Int>_castleLogicPosition;

    private readonly string _playerId;
    private readonly string _rivalPlayerId;
    public LogicTile[][] LogicMap
    {
        get => _logicMap;
        private set => LogicMap = value;
    }
    public int Width { get { return _width; } }
    public int Height { get { return _height; } }
    public MapService(int height, int width,string playerId,string rivalPlayerId)
    {
        _width = width;
        _height = height;
        _playerId = playerId;
        _rivalPlayerId = rivalPlayerId;
        _columnIndexSplit = (height -1)/2;
        _monsterGatePosition = new Vector2Int((width-1)/2, _columnIndexSplit);
        _castleLogicPosition = new Dictionary<string, Vector2Int>  {
            {playerId,new Vector2Int(0, (height-1)/2) }, // TypePlayer.Player,
            {rivalPlayerId, new Vector2Int(width-1, (height-1)/2)} //TypePlayer.Opponent,
        };
        var temp = new Dictionary<TypePlayer, Vector2Int>
        {
            {TypePlayer.Opponent,_castleLogicPosition[playerId]},
            {TypePlayer.Player,_castleLogicPosition[rivalPlayerId]}
            
        };
        _logicMap = new CreateLogicMapService(height, width, _monsterGatePosition, temp).CreateLogicMap(_playerId,_rivalPlayerId).Result;
    }

    private Vector2Int GetRivalCastlePosition(string id)
    {
        return _castleLogicPosition[id];
    }
    public bool IsValidPosition(Vector2Int logicPos, string playerId)
    {
        if (logicPos.y >= 0 && logicPos.y < _width)
        {
            if (_logicMap[logicPos.y][logicPos.x].TypeOfType == TypeTile.Normal
                && _logicMap[logicPos.y][logicPos.x].OwnerId == playerId)
            {
                return true;
            }
        }
        Console.WriteLine("Invalid position");
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
    
    public List<Vector2Int> FindPathForMonster(string playerId , Vector2Int checkPosition)
    {
        BanPosition(checkPosition.x, checkPosition.y);
        List<string> map = new List<string>();
        
        for (int i = 0; i < _height; i++)
        {
            string row = "";
            for (int j = 0; j < _width; j++)
            {
                if (new Vector2Int(j, i) == _monsterGatePosition)
                {
                    row += "A";
                }
                else if (new Vector2Int(j, i) == GetRivalCastlePosition(playerId))
                {
                    row += "B";
                }
                else if (_logicMap[i][j].TypeOfType == TypeTile.Barrier)
                {
                    row += "#";
                }
                else
                {
                    row += " ";
                }
            }
            map.Add(row);
        }
        ReleasePosition(checkPosition.x, checkPosition.y);
        Console.WriteLine($"Check map: {JsonConvert.SerializeObject(map)}");
        var start = new FPTile();
        start.y = map.FindIndex(x => x.Contains("A"));
        start.x = map[start.y].IndexOf("A");

        var finish = new FPTile();
        finish.y = map.FindIndex(x => x.Contains("B"));
        finish.x = map[finish.y].IndexOf("B");

        start.SetDistance(finish.x, finish.y);

        var activeTiles = new List<FPTile> { start };
        var visitedTiles = new List<FPTile>();

        //This is where we created the map from our previous step etc. 

        while (activeTiles.Any())
        {
            var checkTile = activeTiles.OrderBy(x => x.costDistance).First();

            if (checkTile.x == finish.x && checkTile.y == finish.y)
            {
                //We can actually loop through the parents of each tile to find our exact path which we will show shortly.
                var tile = checkTile;
                List<Vector2Int> path = new List<Vector2Int>();
                while (true)
                {
                    path.Insert(0, new Vector2Int(tile.x, tile.y));
                    if (map[tile.y][tile.x] == ' ')
                    {
                        var newMapRow = map[tile.y].ToCharArray();
                        newMapRow[tile.x] = '*';
                        map[tile.y] = new string(newMapRow);
                    }
                    tile = tile.parent;
                    if (tile == null)
                    {
                        return path;
                    }
                }
            }

            visitedTiles.Add(checkTile);
            activeTiles.Remove(checkTile);

            var walkableTiles = GetWalkableTiles(map, checkTile, finish);

            foreach (var walkableTile in walkableTiles)
            {
                //We have already visited this tile so we don't need to do so again!
                if (visitedTiles.Any(x => x.x == walkableTile.x && x.y == walkableTile.y))
                    continue;

                //It's already in the active list, but that's OK, maybe this new tile has a better value (e.g. We might
                //zigzag earlier but this is now straighter). 
                if (activeTiles.Any(x => x.x == walkableTile.x && x.y == walkableTile.y))
                {
                    var existingTile = activeTiles.First(x => x.x == walkableTile.x && x.y == walkableTile.y);
                    if (existingTile.costDistance > checkTile.costDistance)
                    {
                        activeTiles.Remove(existingTile);
                        activeTiles.Add(walkableTile);
                    }
                }
                else
                {
                    //We've never seen this tile before so add it to the list. 
                    activeTiles.Add(walkableTile);
                }
            }
        }

        return new List<Vector2Int>();
    }
    private List<FPTile> GetWalkableTiles(List<string> map, FPTile currentTile, FPTile targetTile)
    {
        List<FPTile> possibleTiles;

        if (currentTile.x == _columnIndexSplit)
        {
            possibleTiles = new List<FPTile>()
            {
                new FPTile { x = currentTile.x + 1, y = currentTile.y, parent = currentTile, cost = currentTile.cost + 1 },
                new FPTile { x = currentTile.x - 1, y = currentTile.y, parent = currentTile, cost = currentTile.cost + 1 }
            };
            
        }
        else
        {
            possibleTiles = new List<FPTile>() {
                new FPTile { x = currentTile.x, y = currentTile.y - 1, parent = currentTile, cost = currentTile.cost + 1 },
                new FPTile { x = currentTile.x, y = currentTile.y + 1, parent = currentTile, cost = currentTile.cost + 1 },
                new FPTile { x = currentTile.x - 1, y = currentTile.y, parent = currentTile, cost = currentTile.cost + 1 },
                new FPTile { x = currentTile.x + 1, y = currentTile.y, parent = currentTile, cost = currentTile.cost + 1 }
            };
        }

        possibleTiles.ForEach(tile => tile.SetDistance(targetTile.x, targetTile.y));

        var maxX = map.First().Length - 1;
        var maxY = map.Count - 1;

        return possibleTiles
            .Where(tile => tile.x >= 0 && tile.x <= maxX)
            .Where(tile => tile.y >= 0 && tile.y <= maxY)
            .Where(tile => map[tile.y][tile.x] == ' ' || map[tile.y][tile.x] == 'B')
            .ToList();
    }

    public void BanPosition(int x, int y)
    {
        _logicMap[y][x].TypeOfType = TypeTile.Barrier;
    }

    public void ReleasePosition(int x, int y)
    {
        _logicMap[y][x].TypeOfType = TypeTile.Normal;

    }
}