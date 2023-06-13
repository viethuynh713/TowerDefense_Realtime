using System.Text;
using Game_Realtime.Hubs;
using Game_Realtime.Model;
using Game_Realtime.Model.Data;
using Microsoft.AspNetCore.SignalR;
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
        _inGameKey = new object();
        // _hubContext.Clients.All.ReceiveMessage("Server", "InitGameSession");
    }

    public async Task<GameSessionModel> CreateNewGameSession(UserMatchingModel playerA, UserMatchingModel playerB)
    {
        PlayerModel playerModelA = new PlayerModel(playerA.userId, playerA.cards);

        PlayerModel playerModelB = new PlayerModel(playerB.userId, playerB.cards);
 

        string gameId = Guid.NewGuid().ToString();

        await _hubContext.Groups.AddToGroupAsync(playerA.contextId, gameId);
        await _hubContext.Groups.AddToGroupAsync(playerB.contextId, gameId);

        GameSessionModel newGameSessionModel = new GameSessionModel(gameId, playerA.gameMode, playerModelA, playerModelB);
    
        lock(_inGameKey)
        {
            _gameSessionModels.Add(gameId, newGameSessionModel);
        }
        await _hubContext.Clients.Groups(gameId).OnStartGame( Encoding.UTF8.GetBytes(gameId));

        return newGameSessionModel;
    }

    public Task CastleHpLost(string gameId, string userId, CastleHPLostData data)
    {
        throw new NotImplementedException();
    }

    public Task PlaceCard(string gameId, string userId, PlaceCardData data)
    {
        throw new NotImplementedException();
    }

    public Task OnEndGame(string gameId)
    {
        throw new NotImplementedException();
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
    Task CastleHpLost(string gameId, string userId, CastleHPLostData data);
    Task PlaceCard(string gameId, string userId, PlaceCardData data);

    Task OnEndGame(string gameId);
}