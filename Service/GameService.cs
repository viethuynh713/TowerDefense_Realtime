using System.Text;
using Game_Realtime.Hubs;
using Game_Realtime.Model;
using Game_Realtime.Model.Data;
using Game_Realtime.Model.Data.DataSend;
using Game_Realtime.Model.InGame;
using Microsoft.AspNetCore.SignalR;
using Networking_System.Model.Data.DataReceive;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Game_Realtime.Service;

public class GameService : IGameService
{
    private readonly Dictionary<string, GameSessionModel> _gameSessionModels;
    private readonly IHubContext<MythicEmpireHub, IMythicEmpireHub> _hubContext;
    private readonly object _inGameKey;

    public GameService(IHubContext<MythicEmpireHub, IMythicEmpireHub> hubContext)
    {
        _hubContext = hubContext;
        _gameSessionModels = new Dictionary<string, GameSessionModel>();
        _inGameKey = new object();

    }

    public async Task CreateAdventureGame(UserMatchingModel player)
    {
        PlayerModel playerModel = new PlayerModel(player.userId, player.cards, player.contextId);
        
        string gameId = Guid.NewGuid().ToString();

        await _hubContext.Groups.AddToGroupAsync(player.contextId, gameId);
        
        GameSessionModel newGameSessionModel = new GameSessionModel(gameId, playerModel, _hubContext);

        lock(_inGameKey)
        {
            _gameSessionModels.Add(gameId, newGameSessionModel);
        }
        
        await _hubContext.Clients.Groups(gameId).OnStartGame(Encoding.UTF8.GetBytes(gameId));
        Console.WriteLine($"\nCreate arena game success: {gameId}");

    }

    public async Task GetGameInfo(string gameId, string senderId, string contextConnectionId)
    {
        if (!_gameSessionModels.ContainsKey(gameId)) return;
        var map = _gameSessionModels[gameId].GetMap();
        List<string> cards = _gameSessionModels[gameId].GetCard(senderId);
        var mode = _gameSessionModels[gameId].GetMode();

        GameInfoSenderData data = new GameInfoSenderData()
        {
            gameId = gameId,
            map = map,
            myListCard = cards,
            mode = mode
        };
        var jsonData = JsonConvert.SerializeObject(data);
        await _hubContext.Clients.Clients(contextConnectionId).OnGameInfo(Encoding.UTF8.GetBytes(jsonData));

    }

    public async Task UpdateMonsterPosition(string gameId, UpdateMonsterPositionData data)
    {
        if (!_gameSessionModels.ContainsKey(gameId)) return;
        if (!_gameSessionModels[gameId].HasPlayer(data.ownerId)) return;

        await _gameSessionModels[gameId].UpdateMonsterPosition(data);
    }

    public async Task AddEnergy(string gameId, AddEnergyData data)
    {
        if (!_gameSessionModels.ContainsKey(gameId)) return;
        if (!_gameSessionModels[gameId].HasPlayer(data.ownerId)) return;

        await _gameSessionModels[gameId].AddEnergy(data);
    }

    public async Task HandlePlayerQuitGame(string gameId, string senderId)
    {
        if (!_gameSessionModels.ContainsKey(gameId)) return;
        if (!_gameSessionModels[gameId].HasPlayer(senderId)) return;
        var player = _gameSessionModels[gameId].GetPlayer(senderId);
        if (player != null)
        {
            var playerWin = _gameSessionModels[gameId].GetRivalPlayer(senderId)?.userId;
            if (playerWin != null)
                await OnEndGame(gameId, playerWin);

            await _hubContext.Clients.Clients(player.ContextId).QuitGame();
        }

    }

    public async Task CreateArenaGame(UserMatchingModel playerA, UserMatchingModel playerB)
    {
        PlayerModel playerModelA = new PlayerModel(playerA.userId, playerA.cards, playerA.contextId);

        PlayerModel playerModelB = new PlayerModel(playerB.userId, playerB.cards, playerB.contextId);
 

        string gameId = Guid.NewGuid().ToString();

        await _hubContext.Groups.AddToGroupAsync(playerA.contextId, gameId);
        await _hubContext.Groups.AddToGroupAsync(playerB.contextId, gameId);

        GameSessionModel newGameSessionModel = new GameSessionModel(gameId, playerModelA, playerModelB, _hubContext);
        
    
        lock(_inGameKey)
        {
            _gameSessionModels.Add(gameId, newGameSessionModel);
        }
        await _hubContext.Clients.Groups(gameId).OnStartGame(Encoding.UTF8.GetBytes(gameId));
        
        Console.WriteLine($"\nCreate arena game success: {gameId}");
    }

    public async Task CastleTakeDamage(string gameId, string senderId, CastleTakeDamageData data)
    {
        if (!_gameSessionModels.ContainsKey(gameId)) return;
        if (!_gameSessionModels[gameId].HasPlayer(senderId)) return;
        
        
        var newCastleHp = _gameSessionModels[gameId].CastleTakeDamage(data).Result;
        if (newCastleHp != null)
        {
            if (newCastleHp.Value <= 0)
            {
                await OnEndGame(gameId,data.ownerId);
            }
            
        }
    }
    
    public async Task OnEndGame(string gameId,string playerWin)
    {
        Console.WriteLine("End Game");
        await _gameSessionModels [gameId].EndGame();
        var endGameDataSender = new EndGameDataSender()
        {
            gameId = gameId,
            playerWin = playerWin,
            totalTime = _gameSessionModels[gameId].GetTotalTime().Result
        };
        var jsonData = JsonConvert.SerializeObject(endGameDataSender);
        await _hubContext.Clients.Groups(gameId).OnEndGame(Encoding.UTF8.GetBytes(jsonData));
        foreach (var player in _gameSessionModels[gameId].GetAllPlayer())
        {
            if(player.Value is PlayerModel)
                await _hubContext.Groups.RemoveFromGroupAsync(((PlayerModel)player.Value).ContextId, gameId);
        }
        _gameSessionModels.Remove(gameId);
    }

    public async Task UpdateMonsterHp(string gameId, string senderId, MonsterTakeDamageData monsterTakeDamageData)
    {
        if (!_gameSessionModels.ContainsKey(gameId)) return;
        if (!_gameSessionModels[gameId].HasPlayer(senderId)) return;

        await _gameSessionModels[gameId].UpdateMonsterHp(monsterTakeDamageData);


    }

    public async Task BuildTower(string gameId, string senderId, BuildTowerData data)
    {
        if (!_gameSessionModels.ContainsKey(gameId)) return;
        if (!_gameSessionModels[gameId].HasPlayer(senderId)) return;
        var towerModel = await _gameSessionModels[gameId].BuildTower(senderId,data);


    }
    public async Task PlaceSpell(string gameId, string senderId, PlaceSpellData data)
    {
        if (!_gameSessionModels.ContainsKey(gameId)) return;
        if (!_gameSessionModels[gameId].HasPlayer(senderId)) return;
        
        var spellModel = await _gameSessionModels[gameId].PlaceSpell(senderId,data);
        
    }
    public async Task CreateMonster(string gameId, string senderId, CreateMonsterData data)
    {
        if (!_gameSessionModels.ContainsKey(gameId)) return;
        if (!_gameSessionModels[gameId].HasPlayer(senderId)) return;
        var monsterModel = await _gameSessionModels[gameId].CreateMonster(senderId,data);
        
    }
    public async Task UpgradeTower(string gameId, string senderId, UpgradeTowerData data)
    {
        if (!_gameSessionModels.ContainsKey(gameId)) return;
        if (!_gameSessionModels[gameId].HasPlayer(senderId)) return;
        var towerStats = await _gameSessionModels[gameId].UpgradeTower(senderId,data);
        
    }
    public async Task SellTower(string gameId, string senderId, SellTowerData data)
    {
        if (!_gameSessionModels.ContainsKey(gameId)) return;
        if (!_gameSessionModels[gameId].HasPlayer(senderId)) return;
        var towerModel = await _gameSessionModels[gameId].SellTower(senderId,data);
        
    }
    public async Task HandlePlayerDisconnect(string connectionId)
    {
        foreach (var game in _gameSessionModels)
        {
            if (game.Value.HasPlayerByConnectionId(connectionId))
            {
                BasePlayer? player = game.Value.GetPlayerByConnectionId(connectionId);
                if (player != null)
                {
                    var playerWin = game.Value.GetRivalPlayer(player.userId)?.userId;
                    if (playerWin != null)
                        await OnEndGame(game.Key, playerWin);
                }

                break;
            }
        }
    }

    
}

public interface IGameService
{
    Task CreateArenaGame(UserMatchingModel playerA, UserMatchingModel playerB);
    Task CastleTakeDamage(string gameId, string userId, CastleTakeDamageData data);
    Task OnEndGame(string gameId, string playerLose);
    Task UpdateMonsterHp(string gameId, string senderId, MonsterTakeDamageData monsterTakeDamageData);
    Task BuildTower(string gameId, string senderId, BuildTowerData buildTowerData);
    Task PlaceSpell(string gameId, string senderId, PlaceSpellData placeSpellData);
    Task CreateMonster(string gameId, string senderId, CreateMonsterData createMonsterData);
    Task UpgradeTower(string gameId, string senderId, UpgradeTowerData upgradeTowerData);
    Task SellTower(string gameId, string senderId, SellTowerData data);
    Task HandlePlayerDisconnect(string connectionId);
    Task CreateAdventureGame(UserMatchingModel newUserMatchingModel);
    Task GetGameInfo(string gameId, string senderId, string contextConnectionId);
    Task UpdateMonsterPosition(string gameId, UpdateMonsterPositionData data);
    Task AddEnergy(string gameId, AddEnergyData addEnergyData);
    Task HandlePlayerQuitGame(string gameId, string senderId);
}