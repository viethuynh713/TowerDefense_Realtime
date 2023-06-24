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
    private readonly IUserMatchingService _userMatchingService;
    private readonly IHubContext<MythicEmpireHub, IMythicEmpireHub> _hubContext;
    private readonly object _inGameKey;

    public GameService(IHubContext<MythicEmpireHub, IMythicEmpireHub> hubContext, IUserMatchingService userMatchingService)
    {
        _userMatchingService = userMatchingService;
        _hubContext = hubContext;
        _gameSessionModels = new Dictionary<string, GameSessionModel>();
        _gameSessionModels.Add("admin-test",new GameSessionModel(
            "admin-test",
            ModeGame.Arena,
            new PlayerModel("1",new List<string>()
            {
                "02dc58b9-11b2-4094-aac7-a8f21b114790",
                "acb777a0-4abc-45a9-90c7-15ea01ed3c7b",
                "2b770545-adaf-4cc6-b5dc-60ada0a3c49c",
                "38842417-c8ae-47d7-afdf-af5afd820677",
                "01ab28d7-8362-4747-82db-df118b3bcd47",
                "d4102b9f-4ccc-4754-8303-b78453bbf166",
                "2bf570ee-a62d-4d59-9db4-1e97d519e7ce"
            }),new PlayerModel("2",new List<string>()
            {
                "02dc58b9-11b2-4094-aac7-a8f21b114790",
                "acb777a0-4abc-45a9-90c7-15ea01ed3c7b",
                "2b770545-adaf-4cc6-b5dc-60ada0a3c49c",
                "38842417-c8ae-47d7-afdf-af5afd820677",
                "01ab28d7-8362-4747-82db-df118b3bcd47",
                "d4102b9f-4ccc-4754-8303-b78453bbf166",
                "2bf570ee-a62d-4d59-9db4-1e97d519e7ce"
            }),_hubContext));
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

    public async Task PlaceCard(string gameId, string senderId, PlaceCardData data)
    {
        if (!_gameSessionModels.ContainsKey(gameId)) return;
        if (!_gameSessionModels[gameId].HasPlayer(senderId)) return;
        JObject senderData = new JObject();
        senderData.Add(new JProperty("userid", senderId));
        senderData.Add(new JProperty("cardId", data.cardId));
        senderData.Add(new JProperty("Xposition", data.Xposition));
        senderData.Add(new JProperty("Yposition", data.Yposition));
        
        switch (data.typeOfCard)
        {
            case CardType.SpellCard:
                await _hubContext.Clients.Groups(gameId).PlaceCard(Encoding.UTF8.GetBytes(senderData.ToString()));
                break;
            case CardType.TowerCard:
                await _hubContext.Clients.Groups(gameId).PlaceCard(Encoding.UTF8.GetBytes(senderData.ToString()));
                break;
            case CardType.MonsterCard:
                var monster = await _gameSessionModels[gameId].CreateMonster(senderId,data);
                await _hubContext.Clients.Groups(gameId).PlaceCard(Encoding.UTF8.GetBytes(senderData.ToString()));
                break;
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

    public async Task GetMap(string gameId)
    {
        var map = _gameSessionModels[gameId].GetMap();
        byte[] mapByte = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(map));
        await _hubContext.Clients.Groups(gameId).OnGetMap(mapByte);
    }

    public async Task GetCardInGame(string gameId, string senderId)
    {
        List<string> cards = _gameSessionModels[gameId].GetCard(senderId);
        byte[] cardByte = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(cards));

        await _hubContext.Clients.Groups(gameId).OnGetCards(cardByte);
        
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
    Task PlaceCard(string gameId, string userId, PlaceCardData data);

    Task OnEndGame(string gameId);
    Task MonsterTakeDamage(string gameId, string senderId, MonsterTakeDamageData monsterTakeDamageData);
    Task GetMap(string gameId);
    Task GetCardInGame(string gameId, string senderId);
}