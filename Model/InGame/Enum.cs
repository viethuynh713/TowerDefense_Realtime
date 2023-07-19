namespace Game_Realtime.Model.InGame
{
    public enum ActionId
    {
        None,
        CreateMonster,
        BuildTower,
        PlaceSpell,
        CastleTakeDamage,
        UpdateMonsterHp,
        GetGameInfo,
        UpgradeTower,
        SellTower,
        UpdateMonsterPosition,
        AddEnergy
    }

    public enum CardType
    {
        TowerCard,
        MonsterCard,
        SpellCard,
        None
    }
    public enum TypePlayer
    {
        Player,
        Opponent
    }
    public enum ModeGame
    {
        None,
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
