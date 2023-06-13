using Game_Realtime.Model;
using Microsoft.AspNetCore.SignalR;
using System.Text;
using Newtonsoft.Json;
using Game_Realtime.Model.Data;
using Newtonsoft.Json.Linq;
using Game_Realtime.Model.InGame;
using Game_Realtime.Service;

namespace Game_Realtime.Hubs
{
    
    public class MythicEmpireHub : Hub<IMythicEmpireHub>
    {
        private readonly IUserMatchingService _userMatchingService;
        private readonly IGameService _gameService;

        public MythicEmpireHub(IUserMatchingService userMatchingService, IGameService gameService)
        {
            this._userMatchingService = userMatchingService;
            this._gameService = gameService;
            Console.WriteLine("\n-----------------------IngameHub Init----------------------");
        }
        
        public override Task OnConnectedAsync()
        {
            Console.WriteLine($"\nUser {Context.ConnectionId} connected");
            return Task.CompletedTask;
        }

        public async Task OnReceiveMatchMakingRequest(string userId, byte[] listCard, int gameMode)
        {
            // Deserialize list card
            var jsonCards = Encoding.UTF8.GetString(listCard);
            List<string>? cardsOfUser = JsonConvert.DeserializeObject<List<string>>(jsonCards);
            
            // Create UserMatchingModel
            UserMatchingModel newUserMatchingModel = new UserMatchingModel(userId, Context.ConnectionId, (ModeGame)gameMode, cardsOfUser);
            
            //Find suitable UserMatchingModel in _userMatchingModels
            var rivalPlayer = _userMatchingService.FindRivalPlayer(newUserMatchingModel);

            if (rivalPlayer.Result == null)
            {
                // Not found suitable UserMatchingModel and create new UserMatchingModel then add this to _userMatchingModels
                await _userMatchingService.CreatWaitingQueue(newUserMatchingModel);
                Console.WriteLine($"\nWaiting: {JsonConvert.SerializeObject(newUserMatchingModel)}");
            }
            else
            {
                Console.WriteLine($"\nFind successfully: {JsonConvert.SerializeObject(rivalPlayer.Result)}");
                await _userMatchingService.RemoveWaitingQueue(rivalPlayer.Result);
                var newGameSession = await _gameService.CreateNewGameSession(newUserMatchingModel, rivalPlayer.Result);
                Console.WriteLine($"\nCreate new game success: {JsonConvert.SerializeObject(newGameSession)}");
            }
            
        }
        public async Task OnCancelMatchMakingRequest()
        {
            await _userMatchingService.CancelWaitingQueue(Context.ConnectionId);
            Console.WriteLine($"\nUser {Context.ConnectionId} cancel successfully ");
        }
        public async Task OnListeningData(byte[] packageData)
        {
            string jsonString = Encoding.UTF8.GetString(packageData);

            JToken jTokenData = JToken.Parse(jsonString);

            ActionId actionId = (ActionId) (int)(jTokenData["actionId"] ?? throw new InvalidOperationException()); 

            switch(actionId)
            {
                case ActionId.CastleHpLost:
                    // Calculate hp lose in data
                    var jsonCastleHpLoseData = jTokenData["data"];
                    if (jsonCastleHpLoseData != null)
                    {
                        CastleHPLostData data = JsonConvert.DeserializeObject<CastleHPLostData>(jsonCastleHpLoseData.ToString())!;
                        // await CastleHpLost(jTokenData["gameId"]?.ToString(), jTokenData["senderId"]?.ToString(), data);
                    }
                    break;
                
                case ActionId.PlaceCard:
                    var jsonPlaceCardData = jTokenData["data"];
                    
                    if (jsonPlaceCardData != null)
                    {
                        PlaceCardData data = JsonConvert.DeserializeObject<PlaceCardData>(jsonPlaceCardData.ToString())!;
                        // await PlaceCard(jTokenData["gameId"]?.ToString(), jTokenData["senderId"]?.ToString(), data);
                    }

                    break;
            }
        }
        
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            Console.WriteLine($"\nPlayer {Context.ConnectionId} disconnect");
        }
    }
}
