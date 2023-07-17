namespace Game_Realtime.Hubs;

public interface IMythicEmpireHub
{
    Task NotifyStatus(string message);
    Task OnReceiveMatchMakingSuccess(int timeWaiting);
    Task CancelSuccess();
    Task OnStartGame(byte[] data);

    Task OnGameInfo(byte[] gameData);
    // Task OnGetMap(byte[] map);
    // Task OnGetCards(byte[] cards);
    Task BuildTower(byte[] cardData);
    Task CreateMonster(byte[] cardData);
    Task PlaceSpell(byte[] cardData);
    Task UpdateEnergy(byte[] newEnergy);
    Task UpdateCastleHp(byte[] newCastleHp);
    Task UpdateWaveTime(byte[] time);
    Task SpawnMonsterWave(byte[] data);
    Task KillMonster(byte[] data);
    Task UpdateMonsterHp(byte[] data);
    Task UpgradeTower(byte[] data);
    Task SellTower(byte[] data);
    Task OnEndGame(byte[] gameData);
}