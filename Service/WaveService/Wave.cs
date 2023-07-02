namespace Game_Realtime.Service.WaveService;

public class Wave
{
    public float currentTime;
    public float maxTimeWaiting;
    public List<string> monsterIds;

    public Wave(float maxTimeWaiting, List<string> monsterIds)
    {
        this.currentTime = maxTimeWaiting;
        this.maxTimeWaiting = maxTimeWaiting;
        this.monsterIds = monsterIds;
    }
}