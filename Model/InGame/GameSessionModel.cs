using Game_Realtime.Model.Map;
using Microsoft.AspNetCore.SignalR;

namespace Game_Realtime.Model
{
    public class GameSessionModel
    {
        public string gameId;

        public DateTime startTime;

        public ModeGame modeGame;

        public Dictionary<string, BasePlayer> players;

        public Tile[][]? logicMap;

        
        public GameSessionModel(string gameId, 
            ModeGame modeGame, BasePlayer playerA, BasePlayer playerB)
        {
            this.gameId = gameId;
            this.modeGame = modeGame;
            this.startTime = DateTime.Now;
            this.players = new Dictionary<string, BasePlayer>
            {
                { playerA.userId, playerA }, 
                {playerB.userId, playerB}
            };

        }

    }
    public enum ModeGame
    {
        Adventure,
        Arena
    }
}
