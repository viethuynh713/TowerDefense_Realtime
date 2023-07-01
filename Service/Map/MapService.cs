using Game_Realtime.Model.Map;

namespace Game_Realtime.Service;

public class MapService
{
    private LogicTile[][] logicMap;
    private readonly int _width;
    public LogicTile[][] LogicMap
    {
        get => logicMap;
        private set => LogicMap = value;
    }
    public MapService(int height, int width,string playerId,string rivalPlayerId)
    {
        _width = width;
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
}