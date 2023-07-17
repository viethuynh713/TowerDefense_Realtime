namespace Game_Realtime.Service.WaveService;

public class Wave
{
    public float currentTime;
    public float maxTimeWaiting;
    public List<SystemMonster> monsterIds;

    public Wave(float maxTimeWaiting, List<SystemMonster> monsterIds)
    {
        this.currentTime = maxTimeWaiting;
        this.maxTimeWaiting = maxTimeWaiting;
        this.monsterIds = monsterIds;
    }

    public void UpgradeData()
    {
        this.currentTime = maxTimeWaiting * 0.8f;
        foreach (var monster in monsterIds)
        {
            monster.hp = (int)(1.1*monster.hp);
            monster.energyGainWhenDie = (int)(monster.energyGainWhenDie * 0.8);
        }
        
    }
}

public class SystemMonster
{
    public string monsterId;
    public int hp;
    public int energyGainWhenDie;
}