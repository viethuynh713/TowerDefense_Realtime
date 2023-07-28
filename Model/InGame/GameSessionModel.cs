using System.Text;
using Game_Realtime.Hubs;
using Game_Realtime.Model.Data;
using Game_Realtime.Model.Data.DataSend;
using Game_Realtime.Model.Map;
using Game_Realtime.Service;
using Game_Realtime.Service.WaveService;
using Microsoft.AspNetCore.SignalR;
using Networking_System.Model.Data.DataReceive;
using Newtonsoft.Json;

namespace Game_Realtime.Model.InGame
{
    public class GameSessionModel : IGameSessionModel
    {
        private readonly string _gameId;

        private readonly DateTime _startTime;

        private readonly ModeGame _modeGame;
        
        private readonly Timer _aiActionTimer;
        
        private readonly Dictionary<string, BasePlayer> _players;

        public readonly MapService mapService;
        private readonly ValidatePackageService _validatePackageService;
        
        private readonly IHubContext<MythicEmpireHub, IMythicEmpireHub> _hubContext;


        private readonly Timer _timerUpdateEnergy;

        private readonly WaveService _waveService;

        private readonly Timer _countWave;

        public GameSessionModel(string gameId, PlayerModel playerA, PlayerModel playerB, IHubContext<MythicEmpireHub, IMythicEmpireHub> hubContext)
        {
            _gameId = gameId;
            _modeGame = ModeGame.Arena;
            _hubContext = hubContext;
            _startTime = DateTime.Now;
            _players = new Dictionary<string, BasePlayer>
            {
                { playerA.userId, playerA }, 
                {playerB.userId, playerB}
            };

            mapService = new MapService(11, 21,playerA.userId,playerB.userId);
            _waveService = new WaveService();
            _validatePackageService = new ValidatePackageService();
            _countWave = new Timer(UpdateWave, null, TimeSpan.Zero, TimeSpan.FromSeconds(1));
            _timerUpdateEnergy = new Timer(UpdateEnergy, null, TimeSpan.Zero, TimeSpan.FromSeconds(2));
            

        }

        public GameSessionModel(string gameId, PlayerModel player, IHubContext<MythicEmpireHub, IMythicEmpireHub> hubContext)
        {
            _gameId = gameId;
            _modeGame = ModeGame.Adventure;
            _hubContext = hubContext;
            _startTime = DateTime.Now;
            var ai = new AiModel();
            _players = new Dictionary<string, BasePlayer>
            {
                { player.userId, player }, 
                {ai.userId, ai}
            };

            mapService = new MapService(11, 21,player.userId,ai.userId);
            _waveService = new WaveService();
            _validatePackageService = new ValidatePackageService();
            _countWave = new Timer(UpdateWave, null, TimeSpan.Zero, TimeSpan.FromSeconds(1));
            _timerUpdateEnergy = new Timer(UpdateEnergy, null, TimeSpan.Zero, TimeSpan.FromSeconds(2));
            ai.InitBot(this);
            _aiActionTimer = new Timer(AiAction, null, TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(1));
        }
        

        private void AiAction(object? state)
        {
            
            foreach (var ai in GetAllPlayer())
            {
                if (ai.Value is AiModel model)
                {
                    model.Battle();

                }
            }
        }
        private bool _isPawnWave = false;
        private void UpdateWave(object? state)
        {
            var currentWaveTime = _waveService.UpdateWaveTime(1f);
            var currentWave = _waveService.GetCurrentWave();
            if (_isPawnWave)
            {
                return;
            }
            if (currentWaveTime <= 0)
            {
                _isPawnWave = true;
                var gatePosition = mapService.MonsterGate; 
                foreach (var monster in currentWave.monsterIds)
                {
                    Thread.Sleep(500);
                    CreateMonsterData data = new CreateMonsterData()
                    {
                        cardId = monster.monsterId,
                        Xposition = gatePosition.x,
                        Yposition = gatePosition.y,
                        stats = new MonsterStats()
                        {
                            AttackSpeed = 1,
                            AttackRange = 1,
                            Damage = 99,
                            Energy = 0,
                            EnergyGainWhenDie = monster.energyGainWhenDie,
                            Hp = monster.hp,
                            MoveSpeed = 1,
                        }

                    };
                    
                    foreach (var player in _players)
                    {
                        var m =  player.Value.CreateMonster(data);
                        if (m.Result != null)
                        {
                            Console.WriteLine(JsonConvert.SerializeObject(m.Result));
                            var jsonData = JsonConvert.SerializeObject(m.Result);
                            _hubContext.Clients.Groups(_gameId).SpawnMonsterWave(Encoding.UTF8.GetBytes(jsonData));
                        }
                        if (player.Value is AiModel)
                        {
                            ((AiModel)player.Value).ToggleAutoSummonMonsterCurrently();
                        }
                    }
                }

                _waveService.NextWave();
                _isPawnWave = false;

            }
            else
            {
                _hubContext.Clients.Groups(_gameId)
                    .UpdateWaveTime(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(currentWave)));
            }
        }
        private void UpdateEnergy(object? state)
        {
            // Console.WriteLine("Update Energy");

            foreach (var player in _players)
            {
                int energy = player.Value.AddEnergy(1).Result;
                if (player.Value is PlayerModel)
                {
                    var playerId = GetPlayer(player.Key)?.ContextId;
                    if (playerId != null)
                        _hubContext.Clients.Clients(playerId).UpdateEnergy(Encoding.UTF8.GetBytes(energy.ToString()));
                }
            }
            
        }
        public bool HasPlayer(string userId)
        {
            if (_players.ContainsKey(userId)) return true;
            return false;
        }

        public ModeGame GetMode()
        {
            return _modeGame;
        }
        public Task<int> GetTotalTime()
        {
            TimeSpan timeSpan = DateTime.Now - _startTime;
            return Task.FromResult(timeSpan.Seconds);
            
        }
        public  async Task<int?> CastleTakeDamage(CastleTakeDamageData data)
        {
            var rivalPlayer = GetRivalPlayer(data.ownerId);
            
            if (rivalPlayer == null) return null;
            
            if (!_validatePackageService.ValidCastlePackage(data)) return null;
            
            var newCastleHp = await _players[rivalPlayer.userId].CastleTakeDamage(data.HpLose);
            
            if (newCastleHp <= 0) return newCastleHp;
            
            // Kill monster
            var energyGain =  _players[data.ownerId].KillMonster(data.monsterId);
            await _validatePackageService.KilledMonster(data.monsterId);
            await _hubContext.Clients.Groups(_gameId).KillMonster(Encoding.UTF8.GetBytes(data.monsterId));
            if (energyGain.Result != null) await _players[rivalPlayer.userId].AddEnergy(energyGain.Result.Value);
                
            //Update Energy
            if (rivalPlayer is PlayerModel)
            {
                await _hubContext.Clients.Clients(((PlayerModel)rivalPlayer).ContextId).UpdateEnergy(Encoding.UTF8.GetBytes(rivalPlayer.energy.ToString()));
            }
            // Update hp for player
            CastleTakeDamageSender senderData = new CastleTakeDamageSender()
            {
                indexPackage = data.indexPackage + 1,
                userId = rivalPlayer.userId,
                currentCastleHp = newCastleHp,
                maxCastleHp = GameConfig.GameConfig.MAX_CASTLE_HP
            };
            var jsonSenderData = JsonConvert.SerializeObject(senderData);
            await _hubContext.Clients.Groups(_gameId).UpdateCastleHp(Encoding.UTF8.GetBytes(jsonSenderData));


            return newCastleHp;
        }

        public async Task EndGame()
        {
            await _countWave.DisposeAsync();
            await _timerUpdateEnergy.DisposeAsync();
            await _aiActionTimer.DisposeAsync();
            
        }

        public async Task<MonsterModel?> CreateMonster(string playerId, CreateMonsterData data)
        {
            if (!mapService.IsValidPosition(new Vector2Int(data.Xposition, data.Yposition), playerId)) return null;
            var monsterModel = await (_players[playerId]).CreateMonster(data);
            
            if (monsterModel != null)
            {
                var jsonMonsterModel = JsonConvert.SerializeObject(monsterModel);
                await _hubContext.Clients.Groups(_gameId).CreateMonster(Encoding.UTF8.GetBytes(jsonMonsterModel));
                var player = GetPlayer(playerId);

                if (player != null)
                {
                    await _hubContext.Clients.Clients(player.ContextId).UpdateEnergy(Encoding.UTF8.GetBytes(player.energy.ToString()));
                }

            }

            return monsterModel;
        }

        public async Task<TowerModel?> BuildTower(string playerId, BuildTowerData data)
        {
            if (!mapService.IsValidPosition(new Vector2Int(data.Xposition, data.Yposition), playerId)) return null;

            var paths = mapService.FindPathForMonster(playerId, new Vector2Int(data.Xposition, data.Yposition));
            // if (_players[playerId] is PlayerModel)
            // {
            //     Console.Write("Path: ");
            //     foreach (var tile in paths)
            //     {
            //         Console.Write("(" + tile.x + ", " + tile.y + ") -> ");
            //     }
            //     Console.WriteLine("");
            // }
            if (paths.Count == 0)
            {
                Console.WriteLine($"No monster path");
                return null;
            }

            var tower =  await _players[playerId].BuildTower(data);
            if (tower != null)
            {
                mapService.BanPosition(data.Xposition, data.Yposition);
            }
            if (tower != null)
            {

                var jsonTowerModel = JsonConvert.SerializeObject(tower);
                await _hubContext.Clients.Groups(_gameId).BuildTower(Encoding.UTF8.GetBytes(jsonTowerModel));
                var player = GetPlayer(playerId);

                if (player != null)
                {
                    await _hubContext.Clients.Clients(player.ContextId).UpdateEnergy(Encoding.UTF8.GetBytes(player.energy.ToString()));
                }
            }
            else
            {
                Console.WriteLine("Can't build tower");
            
            }
            return tower;

        }
        

        public async Task<SpellModel?> PlaceSpell(string playerId, PlaceSpellData data)
        {
            var spellModel = await (_players[playerId]).PlaceSpell(data);
            
            if (spellModel != null)
            {
                var jsonSpellModel = JsonConvert.SerializeObject(spellModel);
                await _hubContext.Clients.Groups(_gameId).PlaceSpell(Encoding.UTF8.GetBytes(jsonSpellModel));
                
                var player = GetPlayer(playerId);
                if (player != null)
                {
                    await _hubContext.Clients.Clients(player.ContextId)
                        .UpdateEnergy(Encoding.UTF8.GetBytes(player.energy.ToString()));
                }

            }

            return spellModel;
        }

        public LogicTile[][] GetMap()
        {
            return mapService.LogicMap;
        }

        public List<string> GetCard(string senderId)
        {
            return ((PlayerModel)_players[senderId]).cards;
        }

        
        public PlayerModel? GetPlayer(string senderId)
        {
            if(_players[senderId] is PlayerModel) return (PlayerModel)_players[senderId];
            return null;
        }
        public Dictionary<string, BasePlayer> GetAllPlayer()
        {
            return _players;
        }

        public BasePlayer? GetRivalPlayer(string playerId)
        {
            foreach (var player in _players)
            {
                if (player.Key != playerId) return player.Value;
            }

            return null;
        }

        public async Task UpdateMonsterHp( MonsterTakeDamageData data)
        {
            if(!_validatePackageService.ValidMonsterPackage(data))return;
            var newHp = _players[data.ownerId].UpdateMonsterHp(data);
            if (newHp.Result > 0)
            {
                var senderData = new UpdateMonsterHpDataSender()
                {
                    indexPackage = data.indexPackage +1,
                    monsterId = data.monsterId,
                    currentHp = newHp.Result.Value,
                    
                };
                var jsonSenderData = JsonConvert.SerializeObject(senderData);
                var dataSend = Encoding.UTF8.GetBytes(jsonSenderData);
                await _hubContext.Clients.Groups(_gameId).UpdateMonsterHp(dataSend);
            }
            else
            {
                var energyGain =  _players[data.ownerId].KillMonster(data.monsterId);
                await _validatePackageService.KilledMonster(data.monsterId);
                await _hubContext.Clients.Groups(_gameId).KillMonster(Encoding.UTF8.GetBytes(data.monsterId));
                var rivalPlayer = GetRivalPlayer(data.ownerId);
                if (rivalPlayer != null)
                {
                    if (energyGain.Result != null)
                        await _players[rivalPlayer.userId].AddEnergy(energyGain.Result.Value);
                    if (rivalPlayer is PlayerModel)
                    {
                        await _hubContext.Clients.Clients(((PlayerModel)rivalPlayer).ContextId)
                            .UpdateEnergy(Encoding.UTF8.GetBytes(rivalPlayer.energy.ToString()));
                    }
                }
            }
        }

        public async Task<TowerStats?> UpgradeTower(string senderId, UpgradeTowerData data)
        {
            var tower =  await _players[senderId].UpgradeTower(data.towerId, data.type);

            if (tower != null)
            {
                UpgradeTowerDataSender dataSender = new UpgradeTowerDataSender()
                {
                    towerId = data.towerId,
                    attackSpeed = tower.AttackSpeed,
                    damage = tower.Damage,
                    range = tower.FireRange,
                };
                var jsonDataSender = JsonConvert.SerializeObject(dataSender);
                await _hubContext.Clients.Groups(_gameId).UpgradeTower(Encoding.UTF8.GetBytes(jsonDataSender));
                
                var player = GetPlayer(senderId);
                if (player != null)
                {
                    await _hubContext.Clients.Clients(player.ContextId)
                        .UpdateEnergy(Encoding.UTF8.GetBytes(player.energy.ToString()));
                }
            }

            return tower;
        }

        public async Task<TowerModel> SellTower(string senderId, SellTowerData data)
        {
            var tower = await _players[senderId].SellTower(data.towerId);
            
            mapService.ReleasePosition(tower.XLogicPosition,tower.YLogicPosition);

            var jsonData = JsonConvert.SerializeObject(tower);
            await _hubContext.Clients.Groups(_gameId).SellTower(Encoding.UTF8.GetBytes(jsonData));

            // await _players[senderId].AddEnergy(tower.EnergyGainWhenSell);
            var player = GetPlayer(senderId);

            if (player != null)
                await _hubContext.Clients.Clients(player.ContextId)
                    .UpdateEnergy(Encoding.UTF8.GetBytes(player.energy.ToString()));
            
            return tower;
        }

        public bool HasPlayerByConnectionId(string connectionId)
        {
            foreach (var player in _players)
            {
                if(player.Value is not  PlayerModel) continue;
                if (((PlayerModel)player.Value).ContextId == connectionId)
                {
                    return true;
                }
            }

            return false;
        }

        public BasePlayer? GetPlayerByConnectionId(string connectionId)
        {
            foreach (var player in _players)
            {
                if(player.Value is not  PlayerModel) continue;
                if (((PlayerModel)player.Value).ContextId == connectionId)
                {
                    return player.Value;
                }
            }

            return null;
        }

        public async Task UpdateMonsterPosition(UpdateMonsterPositionData data)
        {
            await _players[data.ownerId].UpdateMonsterPosition(data);
        }

        public async Task AddEnergy(AddEnergyData data)
        {
            await _players[data.ownerId].AddEnergy(data.energy);

            var player = GetPlayer(data.ownerId);

            if (player != null)
                await _hubContext.Clients.Clients(player.ContextId)
                    .UpdateEnergy(Encoding.UTF8.GetBytes(player.energy.ToString()));

        }
    }

    public interface IGameSessionModel
    {
        Task AddEnergy(AddEnergyData data);
        Task UpdateMonsterPosition(UpdateMonsterPositionData data);
        BasePlayer? GetPlayerByConnectionId(string connectionId);
        bool HasPlayerByConnectionId(string connectionId);
        Task<TowerModel> SellTower(string senderId, SellTowerData data);
        Task<TowerStats?> UpgradeTower(string senderId, UpgradeTowerData data);
        Task UpdateMonsterHp(MonsterTakeDamageData data);
        BasePlayer? GetRivalPlayer(string playerId);
        Dictionary<string, BasePlayer> GetAllPlayer();
        PlayerModel? GetPlayer(string senderId);
        List<string> GetCard(string senderId);
        LogicTile[][] GetMap();

        Task<SpellModel?> PlaceSpell(string playerId, PlaceSpellData data);
        Task<MonsterModel?> CreateMonster(string playerId, CreateMonsterData data);
        Task<int?> CastleTakeDamage(CastleTakeDamageData data);
        Task<TowerModel?> BuildTower(string playerId, BuildTowerData data);
        Task<int> GetTotalTime();
        ModeGame GetMode();
        bool HasPlayer(string dataOwnerId);
        Task EndGame();
    }
}
