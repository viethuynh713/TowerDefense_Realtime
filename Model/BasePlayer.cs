using Game_Realtime.Model.Data;
using Game_Realtime.Model.InGame;

namespace Game_Realtime.Model;

public class BasePlayer
    {
        public readonly string userId;

        public int castleHp;

        public int energy;
        
        public Dictionary<string,MonsterModel> _monsters;

        public Dictionary <string,TowerModel> _towers;

        protected BasePlayer() 
        {
            this.userId = Guid.NewGuid().ToString();
            this.castleHp = GameConfig.GameConfig.MAX_CASTLE_HP;
            this.energy = GameConfig.GameConfig.MAX_ENERGY;
            _monsters = new Dictionary<string, MonsterModel>();
            _towers = new Dictionary<string, TowerModel>();
        }

        protected BasePlayer(string userId)
        {
            this.userId = userId;
            this.castleHp = GameConfig.GameConfig.MAX_CASTLE_HP;
            this.energy = GameConfig.GameConfig.MAX_ENERGY;
            _monsters = new Dictionary<string, MonsterModel>();
            _towers = new Dictionary<string, TowerModel>();

        }

        public Task<int> CastleTakeDamage(int damage)
        {
            this.castleHp -= damage;
            return Task.FromResult(this.castleHp);
        }

        public Task<int> AddEnergy(int addedEnergy)
        {
            int oldEnergy = this.energy;
            this.energy += addedEnergy;
            if (this.energy >= GameConfig.GameConfig.MAX_ENERGY)
            {
                this.energy = GameConfig.GameConfig.MAX_ENERGY;
            }
            if (this is AiModel)
            {
                ((AiModel)this).BotGainEnergy(this.energy - oldEnergy);
            }
            return Task.FromResult(this.energy);
        }

        public virtual Task<MonsterModel> CreateMonster(CreateMonsterData data)
        {
            var stats = (data.stats);
            
            if (stats.Energy > energy) return null;
            energy -= stats.Energy;
            
            
            var monster = new MonsterModel(data.cardId,
                stats.Hp, 
                data.Xposition, 
                data.Yposition,
                this.userId,
                stats.EnergyGainWhenDie);
            
            _monsters.Add(monster.monsterId,monster);
            
            // Console.WriteLine($"Monster list : {JsonConvert.SerializeObject(_monsters)}");
            return Task.FromResult(monster);
        }
        public virtual Task<TowerModel?> BuildTower(BuildTowerData data)
        {
            var stats = data.stats;
            
            if (stats.Energy > energy) return Task.FromResult<TowerModel?>(null);
            
            var tower = new TowerModel(data.cardId, 
                data.Xposition, 
                data.Yposition,
                this.userId, 
                (int)(stats.Energy*((float) GameConfig.GameConfig.TOWER_ENERGY_PERCENT/100)),
                data.stats);
            
            energy -= stats.Energy;

            _towers.Add(tower.towerId, tower);

            return Task.FromResult(tower);
        }
        public virtual Task<SpellModel?> PlaceSpell(PlaceSpellData data)
        {
            var stats = data.stats;
            
            if (stats.Energy > energy) return Task.FromResult<SpellModel?>(null);
            
            var spell = new SpellModel(
                data.cardId, 
                data.Xposition, 
                data.Yposition,
                this.userId
            );
            
            energy -= stats.Energy;

            return Task.FromResult(spell);
        }
        public virtual Task<int?> UpdateMonsterHp(MonsterTakeDamageData data)
        {
            if (!_monsters.ContainsKey(data.monsterId)) return Task.FromResult<int?>(null);
            return Task.FromResult<int?>(_monsters[data.monsterId].UpdateHp(data.damage));
        }

        public virtual Task<int?> KillMonster(string monsterId)
        {
            if (!_monsters.ContainsKey(monsterId)) return Task.FromResult<int?>(null);

            var energyGain = _monsters[monsterId].EnergyGainWhenDie;
            
            _monsters.Remove(monsterId);
            
            return Task.FromResult<int?>(energyGain);
        }

        public virtual async Task<TowerModel> SellTower(string towerId)
        {
            var tower = _towers[towerId];
            
            await AddEnergy(tower.EnergyGainWhenSell);
            
            _towers.Remove(tower.towerId);
            
            return tower;

        }

        public virtual Task<TowerStats?> UpgradeTower(string towerId, UpgradeType type)
        {

            if(_towers[towerId].level[type] > GameConfig.GameConfig.MAX_UPGRADE_LEVEL) return Task.FromResult<TowerStats?>(null);
            
            var upgradeEnergy = _towers[towerId].level[type] * GameConfig.GameConfig.ENERGY_UPDATE;
            
            if (energy < upgradeEnergy) return Task.FromResult<TowerStats?>(null);
            
            energy -= upgradeEnergy;
            
            return Task.FromResult(_towers[towerId].Upgrade(type));
        }

        public Task UpdateMonsterPosition(UpdateMonsterPositionData data)
        {
            if(_monsters.ContainsKey(data.monsterId))
                _monsters[data.monsterId].UpdatePosition(data.Xposition, data.YPosition);
            return Task.CompletedTask;
        }

        public virtual List<string> GetMyListCard()
        {
            return new List<string>();
        }
    }