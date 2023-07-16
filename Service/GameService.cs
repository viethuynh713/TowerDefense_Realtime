using System.Text;
using Game_Realtime.Hubs;
using Game_Realtime.Model;
using Game_Realtime.Model.Data;
using Game_Realtime.Model.Data.DataSend;
using Game_Realtime.Model.InGame;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Game_Realtime.Service;

public class GameService : IGameService
{
    private readonly Dictionary<string, GameSessionModel> _gameSessionModels;
    private readonly IHubContext<MythicEmpireHub, IMythicEmpireHub> _hubContext;
    private readonly object _inGameKey;
    private Timer _aiActionTimer;

    public GameService(IHubContext<MythicEmpireHub, IMythicEmpireHub> hubContext)
    {
        _hubContext = hubContext;
        _aiActionTimer = new Timer(AiAction,null,TimeSpan.Zero,TimeSpan.FromSeconds(1));
        _gameSessionModels = new Dictionary<string, GameSessionModel>();
        _inGameKey = new object();

    }

    private void AiAction(object? state)
    {
        foreach (var game in _gameSessionModels)
        {
            if (game.Value.ModeGame == ModeGame.Adventure)
            {
                foreach (var ai in game.Value.GetAllPlayer())
                {
                    if (ai.Value is AiModel)
                    {
                        ((AiModel)ai.Value).Battle();
                        
                    }
                }
            }
        }
    }

    public async Task CreateAdventureGame(UserMatchingModel player)
    {
        PlayerModel playerModel = new PlayerModel(player.userId, player.cards, player.contextId);

        AiModel bot = new AiModel(player.cards);
        
        string gameId = Guid.NewGuid().ToString();

        await _hubContext.Groups.AddToGroupAsync(player.contextId, gameId);
        
        GameSessionModel newGameSessionModel = new GameSessionModel(gameId, player.gameMode, playerModel, bot, _hubContext);

        lock(_inGameKey)
        {
            _gameSessionModels.Add(gameId, newGameSessionModel);
        }
        
        await _hubContext.Clients.Groups(gameId).OnStartGame(Encoding.UTF8.GetBytes(gameId));
        Console.WriteLine($"\nCreate arena game success: {gameId}");

    }

    public async Task GetGameInfo(string gameId, string senderId, string contextConnectionId)
    {
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
        _hubContext.Clients.Clients(contextConnectionId).OnGameInfo(Encoding.UTF8.GetBytes(jsonData));

    }

    public async Task CreateArenaGame(UserMatchingModel playerA, UserMatchingModel playerB)
    {
        PlayerModel playerModelA = new PlayerModel(playerA.userId, playerA.cards, playerA.contextId);

        PlayerModel playerModelB = new PlayerModel(playerB.userId, playerB.cards, playerB.contextId);
 

        string gameId = Guid.NewGuid().ToString();

        await _hubContext.Groups.AddToGroupAsync(playerA.contextId, gameId);
        await _hubContext.Groups.AddToGroupAsync(playerB.contextId, gameId);

        GameSessionModel newGameSessionModel = new GameSessionModel(gameId, playerA.gameMode, playerModelA, playerModelB, _hubContext);
        
    
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
                await OnEndGame(gameId,senderId);
            }
            
        }
    }
    
    public async Task OnEndGame(string gameId,string playerLose)
    {
        Console.WriteLine("End Game");
        await _gameSessionModels [gameId].EndGame();
        var endGameDataSender = new EndGameDataSender()
        {
            gameId = gameId,
            playerLose = playerLose,
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
        if (towerModel != null)
        {

            var jsonTowerModel = JsonConvert.SerializeObject(towerModel);
            await _hubContext.Clients.Groups(gameId).BuildTower(Encoding.UTF8.GetBytes(jsonTowerModel));
            var player = _gameSessionModels[gameId].GetPlayer(senderId);
            
            await _hubContext.Clients.Clients(player.ContextId).UpdateEnergy(Encoding.UTF8.GetBytes(player.energy.ToString()));

        }
        else
        {
            Console.WriteLine("Can't build tower");
            
        }

    }
    public async Task PlaceSpell(string gameId, string senderId, PlaceSpellData data)
    {
        if (!_gameSessionModels.ContainsKey(gameId)) return;
        if (!_gameSessionModels[gameId].HasPlayer(senderId)) return;
        
        var spellModel = await _gameSessionModels[gameId].PlaceSpell(senderId,data);
        if (spellModel != null)
        {
            var jsonSpellModel = JsonConvert.SerializeObject(spellModel);
            await _hubContext.Clients.Groups(gameId).PlaceSpell(Encoding.UTF8.GetBytes(jsonSpellModel));
            var player = _gameSessionModels[gameId].GetPlayer(senderId);
            
            await _hubContext.Clients.Clients(player.ContextId).UpdateEnergy(Encoding.UTF8.GetBytes(player.energy.ToString()));

        }
    }
    public async Task CreateMonster(string gameId, string senderId, CreateMonsterData data)
    {
        if (!_gameSessionModels.ContainsKey(gameId)) return;
        if (!_gameSessionModels[gameId].HasPlayer(senderId)) return;
        var monsterModel = await _gameSessionModels[gameId].CreateMonster(senderId,data);
        if (monsterModel != null)
        {
            var jsonMonsterModel = JsonConvert.SerializeObject(monsterModel);
            await _hubContext.Clients.Groups(gameId).CreateMonster(Encoding.UTF8.GetBytes(jsonMonsterModel));
            var player = _gameSessionModels[gameId].GetPlayer(senderId);
            
            await _hubContext.Clients.Client(player.ContextId).UpdateEnergy(Encoding.UTF8.GetBytes(player.energy.ToString()));

        }
    }
    public async Task UpgradeTower(string gameId, string senderId, UpgradeTowerData data)
    {
        if (!_gameSessionModels.ContainsKey(gameId)) return;
        if (!_gameSessionModels[gameId].HasPlayer(senderId)) return;
        var towerStats = await _gameSessionModels[gameId].UpgradeTower(senderId,data);
        JObject jsonData = new JObject()
        {
            new JProperty("towerId", data.towerId),
            new JProperty("stats", JsonConvert.SerializeObject(towerStats)),
            
        };
        await _hubContext.Clients.Groups(gameId).UpgradeTower(Encoding.UTF8.GetBytes(jsonData.ToString()));

        var player = _gameSessionModels[gameId].GetPlayer(senderId);
            
        await _hubContext.Clients.Clients(player.ContextId).UpdateEnergy(Encoding.UTF8.GetBytes(player.energy.ToString()));
    }
    public async Task SellTower(string gameId, string senderId, SellTowerData data)
    {
        if (!_gameSessionModels.ContainsKey(gameId)) return;
        if (!_gameSessionModels[gameId].HasPlayer(senderId)) return;
        var towerModel = await _gameSessionModels[gameId].SellTower(senderId,data);

        await _hubContext.Clients.Groups(gameId).SellTower(Encoding.UTF8.GetBytes(towerModel.towerId));
        
        var player = _gameSessionModels[gameId].GetPlayer(senderId);

        if (player != null)
            await _hubContext.Clients.Clients(player.ContextId)
                .UpdateEnergy(Encoding.UTF8.GetBytes(player.energy.ToString()));
    }
    public async Task HandlePlayerDisconnect(string connectionId)
    {
        foreach (var game in _gameSessionModels)
        {
            if (game.Value.HasPlayerByConnectionId(connectionId))
            {
                BasePlayer? player = game.Value.GetPlayerByConnectionId(connectionId);
                if (player != null) await OnEndGame(game.Key, player.userId);
                break;
            }
        }
    }
    public async Task<GameSessionModel> GetGameSession(string gameId)
    {
        return _gameSessionModels[gameId];
        
    }


}

public interface IGameService
{
    Task<GameSessionModel> GetGameSession(string gameId);
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
}