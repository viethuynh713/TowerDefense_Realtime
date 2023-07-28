namespace Game_Realtime.Service.AI
{
    public enum BotPlayMode
    {
        ATTACK,
        DEFEND,
        HYBRIC
    }

    public enum FindTowerTypeStrategy
    {
        EXACT,
        PROGRESS
    }

    public enum FindTowerPosStrategy
    {
        CASTLE_TO_GATE,
        GATE_TO_CASTLE,
        //WEAK_TO_STRONG,
        //STRONG_TO_WEAK
    }
}