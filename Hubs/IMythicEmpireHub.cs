namespace Game_Realtime.Hubs;

public interface IMythicEmpireHub
{
    public Task OnReceiveMatchMakingSuccess(int timeWaiting);
    public Task CancelSuccess();
    public Task OnStartGame(byte[] gameId);
    public Task OnEndGame(byte[] gameData);
    public Task BuildTower(byte[] cardData);
    public Task CreateMonster(byte[] cardData);
    public Task PlaceSpell(byte[] cardData);
    public Task UpdateEnergy(byte[] newEnergy);
    public Task UpdateCastleHp(byte[] newCastleHp);
    public Task UpdateWaveTime(byte[] time);
    public Task ReceiveMessage(string user, string message);
    public Task OnGetMap(byte[] map);
    public Task OnGetCards(byte[] cards);

}