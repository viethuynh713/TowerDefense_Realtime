using System.Diagnostics;
using System.Numerics;
using Game_Realtime.Model.Data;
using Game_Realtime.Service;

namespace Game_Realtime.Model
{
    public class MonsterModel
    {
        public string monsterId;
        public string cardId;
        public string ownerId;
        public int monsterHp;
        public int maxHp;
        public float XLogicPosition;
        public float YLogicPosition;
        public int EnergyGainWhenDie;


        public MonsterModel(string cardId, int monsterHp,int x,int y, string ownerId, int energyGainWhenDie)
        {
            this.monsterId = Guid.NewGuid().ToString();
            this.cardId = cardId;
            this.monsterHp = monsterHp;
            this.maxHp = monsterHp;
            this.XLogicPosition = x;
            this.YLogicPosition = y;
            this.ownerId = ownerId;
            EnergyGainWhenDie = energyGainWhenDie;
        }

        public int UpdateHp(int damage)
        {
            monsterHp += damage;
            if (monsterHp > maxHp) monsterHp = maxHp;
            return monsterHp;
        }

        public void UpdatePosition(float x, float y)
        {
            this.XLogicPosition = x;
            this.YLogicPosition = y;

        }
    }
    
}
