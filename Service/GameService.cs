﻿using System.Diagnostics;
using System.Text;
using Game_Realtime.Hubs;
using Game_Realtime.Model;
using Game_Realtime.Model.Data;
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

    public GameService(IHubContext<MythicEmpireHub, IMythicEmpireHub> hubContext, IUserMatchingService userMatchingService)
    {
        // _userMatchingService = userMatchingService;
        _hubContext = hubContext;
        _gameSessionModels = new Dictionary<string, GameSessionModel>();
        _inGameKey = new object();
    }

    public async Task<GameSessionModel> CreateNewGameSession(UserMatchingModel playerA, UserMatchingModel playerB)
    {
        PlayerModel playerModelA = new PlayerModel(playerA.userId, playerA.cards);

        PlayerModel playerModelB = new PlayerModel(playerB.userId, playerB.cards);
 

        string gameId = Guid.NewGuid().ToString();

        await _hubContext.Groups.AddToGroupAsync(playerA.contextId, gameId);
        await _hubContext.Groups.AddToGroupAsync(playerB.contextId, gameId);

        GameSessionModel newGameSessionModel = new GameSessionModel(gameId, playerA.gameMode, playerModelA, playerModelB, _hubContext);
        
    
        lock(_inGameKey)
        {
            _gameSessionModels.Add(gameId, newGameSessionModel);
        }
        await _hubContext.Clients.Groups(gameId).OnStartGame(Encoding.UTF8.GetBytes(gameId));
        
        Console.WriteLine($"\nCreate new game success: {JsonConvert.SerializeObject(newGameSessionModel)}");
        return newGameSessionModel;
    }

    public async Task CastleTakeDamage(string gameId, string senderId, CastleTakeDamageData data)
    {
        if (!_gameSessionModels.ContainsKey(gameId)) return;
        if (!_gameSessionModels[gameId].HasPlayer(senderId)) return;

        int newCastleHp = _gameSessionModels[gameId].CastleTakeDamage(senderId, data.HpLose);

        if (newCastleHp < 0)
        {
            await OnEndGame(gameId);
        }
        else
        {
            JObject senderData = new JObject(
                new JProperty("userid", senderId),
                new JProperty("newCastleHp", newCastleHp)
                );
            await _hubContext.Clients.Groups(gameId).UpdateCastleHp(Encoding.UTF8.GetBytes(senderData.ToString()));
        }
            
    }
    
    public Task OnEndGame(string gameId)
    {
        throw new NotImplementedException();
    }

    public async Task MonsterTakeDamage(string gameId, string senderId, MonsterTakeDamageData monsterTakeDamageData)
    {
        if (!_gameSessionModels.ContainsKey(gameId)) return;
        if (!_gameSessionModels[gameId].HasPlayer(senderId)) return;
        
        
    }

    public async Task GetMap(string gameId, string contextId)
    {
        var map = _gameSessionModels[gameId].GetMap();
        byte[] mapByte = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(map));
        await _hubContext.Clients.Client(contextId).OnGetMap(mapByte);
    }

    public async Task GetCardInGame(string gameId, string senderId,string contextId)
    {
        List<string> cards = _gameSessionModels[gameId].GetCard(senderId);
        byte[] cardByte = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(cards));

        await _hubContext.Clients.Client(contextId).OnGetCards(cardByte);
        
    }

    public async Task BuildTower(string gameId, string senderId, BuildTowerData data)
    {
        if (!_gameSessionModels.ContainsKey(gameId)) return;
        if (!_gameSessionModels[gameId].HasPlayer(senderId)) return;
        // Console.WriteLine("Valid condition");
        var towerModel = await _gameSessionModels[gameId].BuildTower(senderId,data);
        if (towerModel != null)
        {

            var jsonTowerModel = JsonConvert.SerializeObject(towerModel);
            await _hubContext.Clients.Groups(gameId).BuildTower(Encoding.UTF8.GetBytes(jsonTowerModel));
            // await _hubContext.Clients.Groups(gameId).UpdateEnergy()

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
            // await _hubContext.Clients.Groups(gameId).UpdateEnergy()

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
            // await _hubContext.Clients.Groups(gameId).UpdateEnergy()

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
    Task<GameSessionModel> CreateNewGameSession(UserMatchingModel playerA, UserMatchingModel playerB);
    Task CastleTakeDamage(string gameId, string userId, CastleTakeDamageData data);
    Task OnEndGame(string gameId);
    Task MonsterTakeDamage(string gameId, string senderId, MonsterTakeDamageData monsterTakeDamageData);
    Task GetMap(string gameId, string contextId);
    Task GetCardInGame(string gameId, string senderId, string contextId);
    Task BuildTower(string gameId, string senderId, BuildTowerData buildTowerData);
    Task PlaceSpell(string gameId, string senderId, PlaceSpellData placeSpellData);
    Task CreateMonster(string gameId, string senderId, CreateMonsterData createMonsterData);
}