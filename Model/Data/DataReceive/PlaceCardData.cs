using Game_Realtime.Model.InGame;

namespace Game_Realtime.Model.Data;

public class BuildTowerData
{
    public string cardId;
    public int Xposition;
    public int Yposition;
    public TowerStats stats;
}
public class CreateMonsterData
{
    public string cardId;
    public int Xposition;
    public int Yposition;
    public MonsterStats stats;
}
public class PlaceSpellData
{
    public string cardId;
    public float Xposition;
    public float Yposition;
    public SpellStats stats;
}
public class StatsCard
{
    public int Energy;
}
public class TowerStats : StatsCard
{
    public int Damage;
    public float AttackSpeed;
    public float FireRange;
    public float ExploreRange;
    public float BulletSpeed;
}
public class SpellStats : StatsCard
{
    public float Time;
    public float Duration;
    public float DamegePerDuration;
    public float Range;
    public string Detail;
}
public class MonsterStats : StatsCard
{
    public int Hp;
    public float AttackSpeed;
    public float MoveSpeed;
    public float AttackRange;
    public int Damage;
    public int EnergyGainWhenDie;
}