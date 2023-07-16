using System.Diagnostics;
using System.Text;
using Game_Realtime.Hubs;
using Game_Realtime.Model.Data;
using Game_Realtime.Model.Data.DataSend;
using Game_Realtime.Model.InGame;
using Game_Realtime.Model.Map;
using Game_Realtime.Service;
using Game_Realtime.Service.WaveService;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Game_Realtime.Model
{
    public class GameSessionModel
    {
        private string _gameId;

        private DateTime _startTime;

        private ModeGame _modeGame;
        
        public ModeGame ModeGame
        {
            get => _modeGame;
        }

        private readonly Dictionary<string, BasePlayer> _players;
        
        private readonly MapService _mapService;
        private ValidatePackageService _validatePackageService;
        
        private readonly IHubContext<MythicEmpireHub, IMythicEmpireHub> _hubContext;


        private Timer _timerUpdateEnergy;

        private readonly WaveService _waveService;

        private Timer _countWave;

        public GameSessionModel(string gameId, 
            ModeGame modeGame, BasePlayer playerA, BasePlayer playerB, IHubContext<MythicEmpireHub, IMythicEmpireHub> hubContext)
        {
            _gameId = gameId;
            _modeGame = modeGame;
            _hubContext = hubContext;
            _startTime = DateTime.Now;
            _players = new Dictionary<string, BasePlayer>
            {
                { playerA.userId, playerA }, 
                {playerB.userId, playerB}
            };

            _mapService = new MapService(11, 21,playerA.userId,playerB.userId);
            _waveService = new WaveService();
            _validatePackageService = new ValidatePackageService();
            _countWave = new Timer(UpdateWave, null, TimeSpan.Zero, TimeSpan.FromSeconds(1));
            _timerUpdateEnergy = new Timer(UpdateEnergy, null, TimeSpan.Zero, TimeSpan.FromSeconds(2));
        }

        public bool isPawnWave = false;
        private void UpdateWave(object? state)
        {
            var currentWaveTime = _waveService.UpdateWaveTime(1f);
            var currentWave = _waveService.GetCurrentWave();
            if (isPawnWave)
            {
                return;
            }
            if (currentWaveTime <= 0)
            {
                isPawnWave = true;
                var gatePosition = _mapService.MonsterGate; 
                foreach (var monster in currentWave.monsterIds)
                {
                    Thread.Sleep(1000);
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
                    }
                }

                _waveService.NextWave();
                isPawnWave = false;

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
        public async Task<int> GetTotalTime()
        {
            TimeSpan timeSpan = DateTime.Now - _startTime;
            return timeSpan.Seconds;
        }
        public  async Task<int?> CastleTakeDamage(CastleTakeDamageData data)
        {
            var rivalPlayer = GetRivalPlayer(data.ownerId);
            
            if (rivalPlayer == null) return null;
            
            if (!_validatePackageService.ValidCastlePackage(data)) return null;
            
            var newCastleHp = _players[rivalPlayer.userId].CastleTakeDamage(data.HpLose);
            if (newCastleHp.Result > 0)
            {
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
                    currentCastleHp = newCastleHp.Result,
                    maxCastleHp = GameConfig.GameConfig.MAX_CASTLE_HP
                };
                var jsonSenderData = JsonConvert.SerializeObject(senderData);
                await _hubContext.Clients.Groups(_gameId).UpdateCastleHp(Encoding.UTF8.GetBytes(jsonSenderData));
            }
            
            
            return newCastleHp.Result;
        }

        public async Task EndGame()
        {
            await _countWave.DisposeAsync();
            await _timerUpdateEnergy.DisposeAsync();
        }

        public async Task<MonsterModel?> CreateMonster(string playerId, CreateMonsterData data)
        {
            if (!_mapService.IsValidPosition(new Vector2Int(data.Xposition, data.Yposition), playerId)) return null;
            return await (_players[playerId]).CreateMonster(data);
        }

        public async Task<TowerModel?> BuildTower(string playerId, BuildTowerData data)
        {
            if (!_mapService.IsValidPosition(new Vector2Int(data.Xposition, data.Yposition), playerId)) return null;

            var paths = _mapService.FindPathForMonster(playerId, new Vector2Int(data.Xposition, data.Yposition));
            if (paths.Count == 0)
            {
                Console.WriteLine($"No monster path");
                return null;
            }

            var tower =  await _players[playerId].BuildTower(data);
            if (tower != null)
            {
                _mapService.BanPosition(data.Xposition, data.Yposition);
            }
            return tower;
        }

        public async Task<SpellModel?> PlaceSpell(string playerId, PlaceSpellData data)
        {
            return await (_players[playerId]).PlaceSpell(data);
        }

        public LogicTile[][] GetMap()
        {
            return _mapService.LogicMap;
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

        private BasePlayer? GetRivalPlayer(string playerId)
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
            return await _players[senderId].UpgradeTower(data.towerId, data.type);
        }

        public async Task<TowerModel> SellTower(string senderId, SellTowerData data)
        {
            var tower = await _players[senderId].SellTower(data.towerId);
            
            _mapService.ReleasePosition(tower.XLogicPosition,tower.YLogicPosition);
            
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
    }

}
