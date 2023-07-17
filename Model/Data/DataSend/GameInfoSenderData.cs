using Game_Realtime.Model.InGame;
using Game_Realtime.Model.Map;

namespace Game_Realtime.Model.Data.DataSend;

public class GameInfoSenderData
{
    public string gameId;
    public ModeGame mode;
    public List<string> myListCard;
    public LogicTile[][] map;
}