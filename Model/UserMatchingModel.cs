using Game_Realtime.Model.InGame;

namespace Game_Realtime.Model
{
    public class UserMatchingModel
    {
        public string userId;

        public string contextId;

        public List<string> cards;
        
        public ModeGame gameMode;
        
        public int timeWaiting;
        
        public UserMatchingModel(string userId, string contextId, ModeGame mode, List<string> cards)
        {
            this.userId = userId;
            this.contextId = contextId;
            this.cards = cards;
            this.gameMode = mode;
            this.timeWaiting = 0;
            
        }

        
    }
}
