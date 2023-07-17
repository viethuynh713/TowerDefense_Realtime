using Game_Realtime.Model.Data;

namespace Game_Realtime.Model
{
    public class PlayerModel : BasePlayer
    {
        public readonly List<string> cards;
        
        public string ContextId;
        public PlayerModel(string id,List<string> cards, string contextId) : base(id)
        {
            this.cards = cards;

            _monsters = new Dictionary<string,MonsterModel>();

            ContextId = contextId;

            _towers = new Dictionary<string, TowerModel>();
        }
        
    }
    
    
}
