using Game_Realtime.Model.InGame;

namespace Game_Realtime.Model.Data.DataSend;

public class GameSessionModelData
{
    public string gameId { get; set; } = null!;
    public ModeGame mode { get; set; }
    public DateTime startTime { get; set; }
    public DateTime finishTime { get; set; }
    public float totalTime { get; set; }
    public string? playerA { get; set; }
    public string? playerB { get; set; }
    public List<string>? listCardPlayerA { get; set; }
    public List<string>? listCardPlayerB { get; set; }
    public string? playerWin { get; set; }
}