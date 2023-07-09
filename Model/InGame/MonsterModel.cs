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
        public int XLogicPosition;
        public int YLogicPosition;
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
    }
    public class TowerModel
    {
        public readonly string towerId;
        public string cardId;
        public string ownerId;
        public int XLogicPosition;
        public int YLogicPosition;
        public TowerStats stats;
        public int EnergyGainWhenSell;


        public TowerModel(string cardId,int x,int y, string ownerId, int energyGainWhenSell)
        {
            this.towerId = Guid.NewGuid().ToString();
            this.cardId = cardId;
            this.XLogicPosition = x;
            this.YLogicPosition = y;
            this.ownerId = ownerId;
            EnergyGainWhenSell = energyGainWhenSell;
        }

        public TowerStats Upgrade(UpgradeType type)
        {
            switch (type)
            {
                case UpgradeType.Damage:
                    
                    stats.Damage = (int)(stats.Damage * GameConfig.GameConfig.DAMAGE_UPGRADE_PERCENT);
                    
                    break;
                case UpgradeType.Range:
                    
                    stats.FireRange = (int)(stats.FireRange * GameConfig.GameConfig.RANGE_UPGRADE_PERCENT);

                    break;
                case UpgradeType.AttackSpeed:
                    
                    stats.AttackSpeed = (int)(stats.FireRange * GameConfig.GameConfig.ATTACKSPEED_UPGRADE_PERCENT);

                    break;
            }

            return stats;
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
