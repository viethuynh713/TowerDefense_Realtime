using Game_Realtime.Model.Data;
using Game_Realtime.Model.InGame;

namespace Game_Realtime.Model
{
    public class PlayerModel : BasePlayer
    {
        public readonly List<string> cards;
        
        public string ContextId;
        public PlayerModel(string id,List<string> cards, string contextId) : base(id)
        {
            this.cards = cards;
            ContextId = contextId;

        }
        
    }
    
    
}
