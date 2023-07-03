using System.Text;
using Game_Realtime.Hubs;
using Game_Realtime.Model.Data;
using Game_Realtime.Model.InGame;
using Game_Realtime.Model.Map;
using Game_Realtime.Service;
using Game_Realtime.Service.WaveService;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;

namespace Game_Realtime.Model
{
    public class GameSessionModel
    {
        private string _gameId;

        private DateTime _startTime;

        private ModeGame _modeGame;

        private readonly Dictionary<string, BasePlayer> _players;
        
        private readonly MapService _mapService;
        
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
            
            _countWave = new Timer(UpdateWave, null, TimeSpan.Zero, TimeSpan.FromSeconds(1));
            _timerUpdateEnergy = new Timer(UpdateEnergy, null, TimeSpan.Zero, TimeSpan.FromSeconds(2));
        }

        private void UpdateWave(object? state)
        {
            // var currentWaveTime = _waveService.UpdateWaveTime(1f);
            // var currentWave = _waveService.GetCurrentWave();
            //
            // if (currentWaveTime <= 0)
            // {
            //     _hubContext.Clients.Groups(_gameId)
            //         .SpawnWave(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(currentWave.monsterIds)));
            //     _waveService.NextWave();
            // }
            // else
            // {
            //     _hubContext.Clients.Groups(_gameId)
            //         .UpdateWaveTime(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(currentWave)));
            // }
        }
        private void UpdateEnergy(object? state)
        {

            foreach (var player in _players)
            {
                int energy = player.Value.AddEnergy(1);
                var playerId = GetPlayer(player.Key).ContextId;
                _hubContext.Clients.Clients(playerId).UpdateEnergy(Encoding.UTF8.GetBytes(energy.ToString()));
            }
            
        }
        public bool HasPlayer(string userId)
        {
            if (_players.ContainsKey(userId)) return true;
            return false;
        }

        public int CastleTakeDamage(string senderId, int dataHpLose)
        {
            return _players[senderId].CastleTakeDamage(dataHpLose);
        }

        public void EndGame()
        {
            throw new NotImplementedException();
        }

        public async Task<MonsterModel?> CreateMonster(string playerId, CreateMonsterData data)
        {
            if (!_mapService.IsValidPosition(new Vector2Int(data.Xposition, data.Yposition), playerId)) return null;
            return await ((PlayerModel)_players[playerId]).CreateMonster(data);
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

            var tower =  await ((PlayerModel)_players[playerId]).BuildTower(data);
            if (tower != null)
            {
                _mapService.BanPosition(data.Xposition, data.Yposition);
            }
            return tower;
        }

        public async Task<SpellModel?> PlaceSpell(string playerId, PlaceSpellData data)
        {
            return await ((PlayerModel)_players[playerId]).PlaceSpell(data);
        }

        public LogicTile[][] GetMap()
        {
            return _mapService.LogicMap;
        }

        public List<string> GetCard(string senderId)
        {
            return ((PlayerModel)_players[senderId]).cards;
        }

        
        public PlayerModel GetPlayer(string senderId)
        {
            return (PlayerModel)_players[senderId];
        }
        public Dictionary<string, BasePlayer> GetPlayer()
        {
            return _players;
        }
    }

}
