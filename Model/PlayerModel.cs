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

        // public override async Task<MonsterModel?> CreateMonster(CreateMonsterData data)
        // {
        //     var stats = (data.stats);
        //     
        //     if (stats.Energy > energy) return null;
        //     energy -= stats.Energy;
        //     
        //     
        //     var monster = new MonsterModel(data.cardId,
        //         stats.Hp, 
        //         data.Xposition, 
        //         data.Yposition,
        //         this.userId,
        //         stats.EnergyGainWhenDie);
        //     
        //     _monsters.Add(monster.monsterId,monster);
        //     
        //     // Console.WriteLine($"Monster list : {JsonConvert.SerializeObject(_monsters)}");
        //     return monster;
        // }

        // public override async Task<TowerModel?> BuildTower(BuildTowerData data)
        // {
        //     var stats = data.stats;
        //     
        //     if (stats.Energy > energy) return null;
        //     
        //     var tower = new TowerModel(data.cardId, 
        //         data.Xposition, 
        //         data.Yposition,
        //         this.userId, 
        //         (int)(stats.Energy*(GameConfig.GameConfig.TOWER_ENERGY_PERCENT/100)) );
        //     
        //     energy -= stats.Energy;
        //
        //     _towers.Add(tower.towerId, tower);
        //
        //     return tower;
        // }

        // public override async Task<SpellModel?> PlaceSpell(PlaceSpellData data)
        // {
        //     var stats = data.stats;
        //     
        //     if (stats.Energy > energy) return null;
        //     
        //     var spell = new SpellModel(
        //         data.cardId, 
        //         data.Xposition, 
        //         data.Yposition,
        //         this.userId
        //         );
        //     
        //     energy -= stats.Energy;
        //
        //     return spell;
        // }
        

    }
    
    
}
