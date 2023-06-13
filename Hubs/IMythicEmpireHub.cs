namespace Game_Realtime.Hubs;

public interface IMythicEmpireHub
{
    public Task OnReceiveMatchMakingSuccess(int timeWaiting);
    public Task CancelSuccess();
    public Task OnStartGame(byte[] gameData);

    public Task ReceiveMessage(string user, string message);

}