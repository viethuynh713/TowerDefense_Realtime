using Game_Realtime.Model.InGame;

namespace Game_Realtime.Model.Data;

public class PlaceCardData
{
    public string cardId;
    public CardType typeOfCard;
    public int Xposition;
    public int Yposition;
    public StatsCard stats;
}
public class StatsCard
{
    public int Energy;
}
public class TowerStats : StatsCard
{
    public int Damage;
    public float Range;
    public float AttackSpeed;
    public float FireRange;
    public float ExploreRange;
    public float BulletSpeed;
}
public class SpellStats : StatsCard
{
    public float Duration;
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
}