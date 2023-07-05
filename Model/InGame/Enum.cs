namespace Game_Realtime.Model.InGame
{
    public enum ActionId
    {
        None,
        CreateMonster,
        BuildTower,
        PlaceSpell,
        CastleTakeDamage,
        MonsterTakeDamage,
        GetMyCard,
        GetMap,
        UpgradeTower,
        SellTower
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
    public enum RarityCard
    {
        None,
        Common,
        Rare,
        Mythic,
        Legend

    }
}
