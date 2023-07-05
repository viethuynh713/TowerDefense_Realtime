using System.Numerics;
using Game_Realtime.Model.Data;

namespace Game_Realtime.Model
{
    public class PlayerModel : BasePlayer
    {
        public readonly List<string> cards;
        
        public string ContextId;
        public PlayerModel(string id,List<string> cards, string contextId) : base(id)
        {
            this.cards = cards;

            _monsters = new Dictionary<string,MonsterModel>();

            ContextId = contextId;

            _towers = new Dictionary<string, TowerModel>();
        }

        public override async Task<MonsterModel?> CreateMonster(CreateMonsterData data)
        {
            var stats = (data.stats);
            
            if (stats.Energy > energy) return null;
            
            var monster = new MonsterModel(data.cardId,
                stats.Hp, 
                data.Xposition, 
                data.Yposition,
                this.userId,
                stats.EnergyGainWhenDie);
            
            energy -= stats.Energy;
            
            _monsters.Add(monster.monsterId,monster);

            return monster;
        }

        public override async Task<TowerModel?> BuildTower(BuildTowerData data)
        {
            var stats = data.stats;
            
            if (stats.Energy > energy) return null;
            
            var tower = new TowerModel(data.cardId, 
                data.Xposition, 
                data.Yposition,
                this.userId, 
                (int)(stats.Energy*(GameConfig.GameConfig.TOWER_ENERGY_PERCENT/100)) );
            
            energy -= stats.Energy;

            _towers.Add(tower.towerId, tower);
            
            return tower;
        }

        public override async Task<SpellModel?> PlaceSpell(PlaceSpellData data)
        {
            var stats = data.stats;
            
            if (stats.Energy > energy) return null;
            
            var spell = new SpellModel(data.cardId, 
                data.Xposition, 
                data.Yposition,
                this.userId);
            
            energy -= stats.Energy;

            return spell;
        }
        

    }
    public class AiModel: BasePlayer
    {
        public AiModel() : base() { }

        public async Task<List<string>> ChoseListCard(List<string> rivalCards)
        {
            return null;
        }
    }

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
        public virtual async Task<int> MonsterTakeDamage(string monsterId, int damage)
        {
            return _monsters[monsterId].TakeDamage(damage);
        }

        public virtual async Task<int> KillMonster(string monsterId)
        {
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
}
