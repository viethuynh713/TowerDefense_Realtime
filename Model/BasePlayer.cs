using Game_Realtime.Model.Data;

namespace Game_Realtime.Model;

public class BasePlayer
    {
        public readonly string userId;

        public int castleHp;

        public int energy;
        
        protected Dictionary<string,MonsterModel> _monsters;

        protected Dictionary <string,TowerModel> _towers;

        protected Dictionary<UpgradeType, int> countUpgrade;
        protected BasePlayer() 
        {
            this.userId = Guid.NewGuid().ToString();
            this.castleHp = GameConfig.GameConfig.MAX_CASTLE_HP;
            this.energy = GameConfig.GameConfig.MAX_ENERGY;
            _monsters = new Dictionary<string, MonsterModel>();
            _towers = new Dictionary<string, TowerModel>();
            this.countUpgrade = new Dictionary<UpgradeType, int>()
            {
                { UpgradeType.Damage ,1},
                { UpgradeType.Range ,1},
                { UpgradeType.AttackSpeed ,1},
            };
        }

        protected BasePlayer(string userId)
        {
            this.userId = userId;
            this.castleHp = GameConfig.GameConfig.MAX_CASTLE_HP;
            this.energy = GameConfig.GameConfig.MAX_ENERGY;
            _monsters = new Dictionary<string, MonsterModel>();
            _towers = new Dictionary<string, TowerModel>();
            this.countUpgrade = new Dictionary<UpgradeType, int>()
            {
                { UpgradeType.Damage ,1},
                { UpgradeType.Range ,1},
                { UpgradeType.AttackSpeed ,1},
            };
        }

        public async Task<int> CastleTakeDamage(int damage)
        {
            this.castleHp -= damage;
            return this.castleHp;
        }

        public async Task<int> AddEnergy(int addedEnergy)
        {
            this.energy += addedEnergy;
            if (this.energy >= GameConfig.GameConfig.MAX_ENERGY)
            {
                this.energy = GameConfig.GameConfig.MAX_ENERGY;
            }
            return this.energy;
        }

        public virtual async Task<MonsterModel?> CreateMonster(CreateMonsterData data)
        {
            return null;
        }
        public virtual async  Task<TowerModel?> BuildTower(BuildTowerData data)
        {
            return null;
        }
        public virtual async Task<SpellModel?> PlaceSpell(PlaceSpellData data)
        {
            return null;
        }
        public virtual async Task<int?> UpdateMonsterHp(MonsterTakeDamageData data)
        {
            if (!_monsters.ContainsKey(data.monsterId)) return null;
            return _monsters[data.monsterId].UpdateHp(data.damage);
        }

        public virtual async Task<int?> KillMonster(string monsterId)
        {
            if (!_monsters.ContainsKey(monsterId)) return null;

            var energyGain = _monsters[monsterId].EnergyGainWhenDie;
            
            _monsters.Remove(monsterId);
            
            return energyGain;
        }

        public virtual async Task<TowerModel> SellTower(string towerId)
        {
            var tower = _towers[towerId];
            
            await AddEnergy(tower.EnergyGainWhenSell);
            
            _towers.Remove(tower.towerId);
            
            return tower;

        }

        public virtual async Task<TowerStats?> UpgradeTower(string towerId, UpgradeType type)
        {
            if (countUpgrade[type] > GameConfig.GameConfig.MAX_UPGRADE_LEVEL) return null;
            
            var upgradeEnergy = countUpgrade[type] * GameConfig.GameConfig.ENERGY_UPDATE;
            
            if (energy < upgradeEnergy) return null;
            
            energy -= upgradeEnergy;
            
            return _towers[towerId].Upgrade(type);
        }
    }