namespace Game_Realtime.Model
{
    public class UserMatchingModel
    {
        public string userId;

        public string contextId;

        public List<string>? cards;
        
        public ModeGame gameMode;
        
        public int timeWaiting;
        
        private Timer _timer;
        
        public UserMatchingModel()
        {
        }

        public UserMatchingModel(string userId, string contextId, ModeGame mode, List<string>? cards)
        {
            this.userId = userId;
            this.contextId = contextId;
            this.cards = cards;
            this.gameMode = mode;
            this.timeWaiting = 0;
            
        }

        
    }
}
