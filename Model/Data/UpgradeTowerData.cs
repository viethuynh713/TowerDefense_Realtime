namespace Game_Realtime.Model.Data;

public class UpgradeTowerData
{
    public string towerId;
    public UpgradeType type;
}

public enum UpgradeType
{
    Damage,
    Range,
    AttackSpeed
    
}