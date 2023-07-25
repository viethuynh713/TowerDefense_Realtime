namespace Game_Realtime.Service;

public class OnlineService : IOnlineService
{
    public async Task<bool> IsOnline()
    {
        return false;
    }

    public async Task Offline()
    {
        
    }
}

public interface IOnlineService
{ 
    Task<bool> IsOnline();
    Task Offline();

}