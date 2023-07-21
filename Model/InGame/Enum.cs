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
        AddEnergy,
        QuitGame
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
