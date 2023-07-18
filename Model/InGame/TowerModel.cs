using Game_Realtime.Model.Data;

namespace Game_Realtime.Model.InGame;

public class TowerModel
{
    public readonly string towerId;
    public string cardId;
    public string ownerId;
    public readonly int XLogicPosition;
    public readonly int YLogicPosition;
    public TowerStats stats;
    public int EnergyGainWhenSell;
    public Dictionary<UpgradeType, int> level;


    public TowerModel(string cardId,int x,int y, string ownerId, int energyGainWhenSell, TowerStats stats)
    {
        this.towerId = Guid.NewGuid().ToString();
        this.cardId = cardId;
        this.XLogicPosition = x;
        this.YLogicPosition = y;
        this.ownerId = ownerId;
        EnergyGainWhenSell = energyGainWhenSell;
        this.stats = stats;
        
        this.level = new Dictionary<UpgradeType, int>()
        {
            { UpgradeType.Damage ,1},
            { UpgradeType.Range ,1},
            { UpgradeType.AttackSpeed ,1},
        };
        
    }

    public TowerStats Upgrade(UpgradeType type)
    {
        switch (type)
        {
            case UpgradeType.Damage:
                    
                stats.Damage = (int)(stats.Damage * GameConfig.GameConfig.DAMAGE_UPGRADE_PERCENT);
                
                break;
            case UpgradeType.Range:
                    
                stats.FireRange = (stats.FireRange * GameConfig.GameConfig.RANGE_UPGRADE_PERCENT);

                break;
            case UpgradeType.AttackSpeed:
                    
                stats.AttackSpeed = (stats.FireRange * GameConfig.GameConfig.ATTACKSPEED_UPGRADE_PERCENT);

                break;
        }

        level[type]++;
        return stats;
    }
}