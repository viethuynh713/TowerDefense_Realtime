using Game_Realtime.Model;
using Microsoft.AspNetCore.SignalR;
using System.Text;
using Newtonsoft.Json;
using Game_Realtime.Model.Data;
using Newtonsoft.Json.Linq;
using Game_Realtime.Model.InGame;
using Game_Realtime.Service;
using Game_Realtime.Service.WaveService;

namespace Game_Realtime.Hubs
{
    
    public class MythicEmpireHub : Hub<IMythicEmpireHub>
    {
        private readonly IUserMatchingService _userMatchingService;
        private readonly IGameService _gameService;
        
        public MythicEmpireHub(IUserMatchingService userMatchingService, IGameService gameService)
        {
            _userMatchingService = userMatchingService;
            _gameService = gameService;
            // Dictionary<int, Wave> waves = new Dictionary<int, Wave>();
            // Console.WriteLine(JsonConvert.SerializeObject(waves));
            Console.WriteLine("\n-----------------------InGameHub Init----------------------");
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
           
            var  mode = (ModeGame)Enum.Parse(typeof(ModeGame), gameMode.ToString());
            // Create UserMatchingModel
                if (cardsOfUser != null)
                {
                    UserMatchingModel newUserMatchingModel = new UserMatchingModel(userId, Context.ConnectionId, mode, cardsOfUser);
                    
                    if (mode == ModeGame.Arena)
                    {
                        //Find suitable UserMatchingModel in _userMatchingModels
                        var rivalPlayer = _userMatchingService.FindRivalPlayer(newUserMatchingModel);

                        if (rivalPlayer.Result == null)
                        {
                            // Not found suitable UserMatchingModel and create new UserMatchingModel then add this to _userMatchingModels
                            await _userMatchingService.AddPlayerToWaitingQueue(newUserMatchingModel);
                            Console.WriteLine($"\nWaiting: {JsonConvert.SerializeObject(newUserMatchingModel)}");
                        }
                        else
                        {
                            Console.WriteLine(
                                $"\nFind successfully: {JsonConvert.SerializeObject(rivalPlayer.Result)}");
                            await _userMatchingService.RemovePlayerInWaitingQueue(rivalPlayer.Result);
                            await _gameService.CreateArenaGame(newUserMatchingModel, rivalPlayer.Result);

                        }
                    }
                    else
                    {
                        await _gameService.CreateAdventureGame(newUserMatchingModel);
                    }
                }
        }
        public async Task OnCancelMatchMakingRequest()
        {
            await _userMatchingService.CancelWaitingQueue(Context.ConnectionId);
        }
        public async Task OnListeningData(byte[] packageData)
        {
            string jsonString = Encoding.UTF8.GetString(packageData);

            JObject  jObjData = JObject.Parse(jsonString);

            ActionId actionId = (ActionId) (int)(jObjData["actionId"] ?? throw new InvalidOperationException());
            
            string gameId = (jObjData["gameId"]?.ToString() ?? throw new InvalidOperationException());
            
            string senderId = (jObjData["senderId"]?.ToString() ?? throw new InvalidOperationException());
            
            var data = jObjData["data"]! ?? throw new InvalidOperationException();
            Console.WriteLine(actionId);
            switch(actionId)
            {
                case ActionId.CastleTakeDamage:
                    CastleTakeDamageData castleTakeDamageData = JsonConvert.DeserializeObject<CastleTakeDamageData>(data.ToString())!;
                    
                    await _gameService.CastleTakeDamage(gameId, senderId, castleTakeDamageData);

                    break;

                case ActionId.BuildTower:
                    BuildTowerData buildTowerData = JsonConvert.DeserializeObject<BuildTowerData>(data.ToString())!;
                    await _gameService.BuildTower(gameId, senderId, buildTowerData);
                    break;
                
                case ActionId.PlaceSpell:
                    PlaceSpellData placeSpellData = JsonConvert.DeserializeObject<PlaceSpellData>(data.ToString())!;
                    await _gameService.PlaceSpell(gameId, senderId, placeSpellData);
                    break;
                
                case ActionId.CreateMonster:
                    CreateMonsterData createMonsterData = JsonConvert.DeserializeObject<CreateMonsterData>(data.ToString())!;
                    await _gameService.CreateMonster(gameId, senderId, createMonsterData);
                    break;
                
                case ActionId.UpdateMonsterHp:
                    MonsterTakeDamageData monsterTakeDamageData = JsonConvert.DeserializeObject<MonsterTakeDamageData>(data.ToString())!;
                    
                    await _gameService.UpdateMonsterHp(gameId, senderId, monsterTakeDamageData);
                    
                    break;
                case ActionId.UpgradeTower:
                    UpgradeTowerData upgradeTowerData = JsonConvert.DeserializeObject<UpgradeTowerData>(data.ToString())!;
                    await _gameService.UpgradeTower(gameId, senderId, upgradeTowerData);

                    break;
                case ActionId.SellTower:
                    SellTowerData sellTowerData = JsonConvert.DeserializeObject<SellTowerData>(data.ToString())!;
                    await _gameService.SellTower(gameId, senderId, sellTowerData);
                    break;
                    
                case ActionId.GetGameInfo:

                    await _gameService.GetGameInfo(gameId, senderId, Context.ConnectionId);
                    
                    break;

            }
        }
        
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            Console.WriteLine($"\nPlayer {Context.ConnectionId} disconnect");
            await _userMatchingService.CancelWaitingQueue(Context.ConnectionId);
            await _gameService.HandlePlayerDisconnect(Context.ConnectionId);
        }
    }
}
