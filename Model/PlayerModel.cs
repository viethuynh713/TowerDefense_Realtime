using System.Numerics;
using Game_Realtime.Model.Data;

namespace Game_Realtime.Model
{
    public class PlayerModel : BasePlayer
    {
        public List<string> cards;

        private List<MonsterModel> monsters;
        
        

        public PlayerModel(string id,List<string> cards) : base(id)
        {
            this.cards = cards;

            this.monsters = new List<MonsterModel>();
        }

        public async Task<MonsterModel?> CreateMonster(CreateMonsterData data)
        {
            var stats = (data.stats);
            
            if (stats.Energy > energy) return null;
            
            var monster = new MonsterModel(data.cardId, stats.Hp, data.Xposition, data.Yposition,this.userId);
            
            energy -= stats.Energy;
            
            monsters.Add(monster);

            return monster;
        }

        public async Task<TowerModel?> BuildTower(BuildTowerData data)
        {
            var stats = data.stats;
            // Console.WriteLine($"Current Energy: {energy} || towerEnergy: {data.stats.Energy}");
            if (stats.Energy > energy) return null;
            
            var tower = new TowerModel(data.cardId, data.Xposition, data.Yposition,this.userId);
            
            energy -= stats.Energy;

            return tower;
        }

        public async Task<SpellModel?> PlaceSpell(PlaceSpellData data)
        {
            var stats = data.stats;
            
            if (stats.Energy > energy) return null;
            
            var spell = new SpellModel(data.cardId, data.Xposition, data.Yposition,this.userId);
            
            energy -= stats.Energy;

            return spell;
        }
    }
    public class AiModel: BasePlayer
    {
        public AiModel() : base() { }
    }

    public class BasePlayer
    {
        public string userId;

        public int castleHp;

        public int energy;

        protected BasePlayer() 
        {
            this.userId = Guid.NewGuid().ToString();
            this.castleHp = 1000;
            this.energy = 100;
        }

        protected BasePlayer(string userId)
        {
            this.userId = userId;
            this.castleHp = 1000;
            this.energy = 100;
        }

        public int CastleTakeDamage(int damage)
        {
            this.castleHp -= damage;
            return this.castleHp;
        }

        public void UpdateEnergy(int energy)
        {
            this.energy -= energy;
        }


    }
}
