namespace Game_Realtime.Service;

public class FPTile
{
    public int x { get; set; }
    public int y { get; set; }
    public int cost { get; set; }
    public int distance { get; set; }
    
    public int costDistance => cost + distance;
    
    public FPTile parent { get; set; }

    //The distance is essentially the estimated distance, ignoring walls to our target. 
    //So how many tiles left and right, up and down, ignoring walls, to get there. 
    public void SetDistance(int targetX, int targetY)
    {
        distance = Math.Abs(targetX - x) + Math.Abs(targetY - y);
    }
}