using Game_Realtime.Hubs;
using Game_Realtime.Model;
using Microsoft.AspNetCore.SignalR;

namespace Game_Realtime.Service;

public class UserMatchingService : IUserMatchingService
{
    private List<UserMatchingModel> _userMatchingModels;
    private readonly IHubContext<MythicEmpireHub, IMythicEmpireHub> _hubContext;
    private readonly object _userMatchingKey;
    private Timer _countTimer;
    public UserMatchingService(IHubContext<MythicEmpireHub, IMythicEmpireHub> hubContext)
    {
        _countTimer = new Timer(UpdateTime, null, TimeSpan.Zero, TimeSpan.FromSeconds(1));
        _userMatchingModels = new List<UserMatchingModel>();
        _userMatchingKey = new object();
        _hubContext = hubContext;
    }

    private void UpdateTime(object? state)
    {
        foreach (var model in _userMatchingModels)
        {
            model.timeWaiting++;
            _hubContext.Clients.Client(model.contextId).OnReceiveMatchMakingSuccess(model.timeWaiting);

        }
    }

    public async Task<UserMatchingModel?> FindRivalPlayer(UserMatchingModel playerInfo)
    {
        return _userMatchingModels.Find(x =>
        {
            return x.gameMode == playerInfo.gameMode;
        });
    }

    public async Task AddPlayerToWaitingQueue(UserMatchingModel playerInfo)
    {
        lock (_userMatchingKey)
        {
            _userMatchingModels.Add(playerInfo);
        }
        // Notify for user and change state to Waiting
        await _hubContext.Clients.Client(playerInfo.contextId).OnReceiveMatchMakingSuccess(0);
    }

    public async Task CancelWaitingQueue(string contextId)
    {
        lock (_userMatchingKey)
        {
            var model = _userMatchingModels.Find(x=>x.contextId == contextId);
            if (model != null)
            {
                _userMatchingModels.Remove(model);
                _hubContext.Clients.Clients(contextId).CancelSuccess();
                Console.WriteLine($"\nRemove player {model.contextId} in waiting queue");

            }
            else
            {
                // TODO : throw 
            }
        }
    }

    public async Task RemovePlayerInWaitingQueue(UserMatchingModel model)
    {
        lock (_userMatchingKey)
        {
            _userMatchingModels.Remove(model);
        }
    }public async Task RemovePlayerInWaitingQueue(string contextId)
    {
        var model = _userMatchingModels.Find(user => user.contextId == contextId);
        if (model == null) return;
        lock (_userMatchingKey)
        {
            _userMatchingModels.Remove(model);
            Console.WriteLine($"\nRemove player {model.contextId} in waiting queue");

        }
    }
}

public interface IUserMatchingService
{
    Task<UserMatchingModel?> FindRivalPlayer(UserMatchingModel playerInfo);
    Task AddPlayerToWaitingQueue(UserMatchingModel playerInfo);
    Task CancelWaitingQueue(string contextId);
    Task RemovePlayerInWaitingQueue(UserMatchingModel model);
    Task RemovePlayerInWaitingQueue(string contextId);
}