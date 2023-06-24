namespace Game_Realtime.Model.InGame
{
    public enum ActionId
    {
        None,
        PlaceCard,
        CastleTakeDamage,
        MonsterTakeDamage,
        GetMyCard,
        GetMap,
    }

    public enum CardType
    {
        None,
        TowerCard,
        MonsterCard,
        SpellCard
    }
    public enum TypePlayer
    {
        Player,
        Opponent
    }
    public enum ModeGame
    {
        Adventure,
        Arena
    }
}
