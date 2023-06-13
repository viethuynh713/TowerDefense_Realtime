using Game_Realtime.Hubs;
using Game_Realtime.Model;
using Microsoft.AspNetCore.SignalR;

namespace Game_Realtime.Service;

public class UserMatchingService : IUserMatchingService
{
    private List<UserMatchingModel> _userMatchingModels;
    private readonly IHubContext<MythicEmpireHub, IMythicEmpireHub> _hubContext;
    private readonly object _userMatchingKey;

    public UserMatchingService(IHubContext<MythicEmpireHub, IMythicEmpireHub> hubContext)
    {
        _userMatchingModels = new List<UserMatchingModel>();
        _userMatchingKey = new object();
        _hubContext = hubContext;
    }

    public async Task<UserMatchingModel?> FindRivalPlayer(UserMatchingModel playerInfo)
    {
        return _userMatchingModels.Find(x =>
        {
            return x.gameMode == playerInfo.gameMode;
        });
        return null;
    }

    public async Task CreatWaitingQueue(UserMatchingModel playerInfo)
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
            }
            else
            {
                // TODO : throw 
            }
        }
    }

    public async Task RemoveWaitingQueue(UserMatchingModel model)
    {
        lock (_userMatchingKey)
        {
            _userMatchingModels.Remove(model);
        }
    }
}

public interface IUserMatchingService
{
    Task<UserMatchingModel?> FindRivalPlayer(UserMatchingModel playerInfo);
    Task CreatWaitingQueue(UserMatchingModel playerInfo);
    Task CancelWaitingQueue(string contextId);
    Task RemoveWaitingQueue(UserMatchingModel model);
}