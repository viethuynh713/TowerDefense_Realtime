using Game_Realtime.Hubs;
using Game_Realtime.Model.Data;
using Game_Realtime.Model.InGame;
using Game_Realtime.Model.Map;
using Game_Realtime.Service;
using Microsoft.AspNetCore.SignalR;

namespace Game_Realtime.Model
{
    public class GameSessionModel
    {
        private string _gameId;

        private DateTime _startTime;

        private ModeGame _modeGame;

        private readonly Dictionary<string, BasePlayer> _players;
        
        private MapService _mapService;
        private readonly IHubContext<MythicEmpireHub, IMythicEmpireHub> _hubContext;

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
            return await ((PlayerModel)_players[playerId]).BuildTower(data);
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

        public bool IsValidPosition(int dataXposition, int dataYposition, string playerId)
        {
            return _mapService.IsValidPosition(new Vector2Int(dataXposition, dataYposition), playerId);
        }
    }

}
