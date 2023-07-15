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
        WEAK_TO_STRONG,
        STRONG_TO_WEAK,
        CASTLE_TO_GATE,
        GATE_TO_CASTLE
    }
}