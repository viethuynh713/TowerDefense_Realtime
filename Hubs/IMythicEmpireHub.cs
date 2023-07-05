namespace Game_Realtime.Hubs;

public interface IMythicEmpireHub
{
    Task ReceiveMessage(string user, string message);
    Task OnReceiveMatchMakingSuccess(int timeWaiting);
    Task CancelSuccess();
    Task OnStartGame(byte[] gameId);
    Task OnEndGame(byte[] gameData);
    Task BuildTower(byte[] cardData);
    Task CreateMonster(byte[] cardData);
    Task PlaceSpell(byte[] cardData);
    Task UpdateEnergy(byte[] newEnergy);
    Task UpdateCastleHp(byte[] newCastleHp);
    Task UpdateWaveTime(byte[] time);
    Task SpawnWave(byte[] data);
    Task OnGetMap(byte[] map);
    Task OnGetCards(byte[] cards);
    Task KillMonster(byte[] data);
    Task UpdateMonsterHp(byte[] data);
    Task UpgradeTower(byte[] data);
    Task SellTower(byte[] data);
}