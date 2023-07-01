using System.Numerics;
using Game_Realtime.Service;

namespace Game_Realtime.Model
{
    public class MonsterModel
    {
        public string monsterId;
        public string cardId;
        public string ownerId;
        public int monsterHp;
        public int XLogicPosition;
        public int YLogicPosition;

        public MonsterModel(string cardId, int monsterHp,int x,int y, string ownerId)
        {
            this.monsterId = Guid.NewGuid().ToString();
            this.cardId = cardId;
            this.monsterHp = monsterHp;
            this.XLogicPosition = x;
            this.YLogicPosition = y;
            this.ownerId = ownerId;
        }
    }
    public class TowerModel
    {
        public string towerId;
        public string cardId;
        public string ownerId;
        public int XLogicPosition;
        public int YLogicPosition;

        public TowerModel(string cardId,int x,int y, string ownerId)
        {
            this.towerId = Guid.NewGuid().ToString();
            this.cardId = cardId;
            this.XLogicPosition = x;
            this.YLogicPosition = y;
            this.ownerId = ownerId;
        }
    }
    public class SpellModel
    {
        public string spellId;
        public string cardId;
        public string ownerId;
        public int XLogicPosition;
        public int YLogicPosition;

        public SpellModel(string cardId,int x,int y, string ownerId)
        {
            this.spellId = Guid.NewGuid().ToString();
            this.cardId = cardId;
            this.XLogicPosition = x;
            this.YLogicPosition = y;
            this.ownerId = ownerId;
        }
    }
}
