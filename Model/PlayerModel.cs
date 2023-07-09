using System.Numerics;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;
using System.IO;
using Game_Realtime.Model.Data;
using Game_Realtime.Model.InGame;
using Game_Realtime.Service.AI;
using Game_Realtime.Service.AI.SelectCardService;
using Game_Realtime.Model.Map;
using Game_Realtime.Service;
using Game_Realtime.Service.AI.TowerBuildingMapService;
using Game_Realtime.Service.AI.BehaviorTree.Bot;

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
            // Console.WriteLine($"Monster list : {JsonConvert.SerializeObject(_monsters)}");
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
            // Console.WriteLine($"Tower list : {JsonConvert.SerializeObject(_towers)}");

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
        private BotPlayMode playMode;
        private List<(CardType, string)> cardSelected;
        private BotLogicTile[][] towerBuildingMap;
        private int towerBuildingMapWidth;
        private int towerBuildingMapHeight;
        private float energyBuildTowerRate;
        private BotBT behavior;
        public AiModel() : base() { }

        public async Task<List<string>> ChooseListCard(List<string> rivalCards)
        {
            playMode = (BotPlayMode)new Random().Next(0, 3);
            // select card by mode
            cardSelected = new List<(CardType, string)>();
            AIConstant.cardSelectingStrategy.TryGetValue(playMode, out var strategy);
            if (strategy != null)
            {
                foreach (var cardTypeStrategy in strategy)
                {
                    AIConstant.cardGroup.TryGetValue(cardTypeStrategy.Key, out var cardListOfType);
                    if (cardListOfType != null)
                    {
                        foreach (var nCardOfType in cardTypeStrategy.Value)
                        {
                            cardListOfType.TryGetValue(nCardOfType.Key, out var cardList);
                            if (cardList != null)
                            {
                                for (int i = 0; i < nCardOfType.Value; i++)
                                {
                                    cardSelected.Add((cardTypeStrategy.Key, cardList[new Random().Next(0, cardList.Length)]));
                                }
                            }
                        }
                    }
                }
            }
            // get card json file
            var json = File.ReadAllText("../CardConfig/MythicEmpire.Cards.json");
            var stockCards = JsonConvert.DeserializeObject<List<MythicEmpireCard>>(json);
            if (stockCards == null) Console.WriteLine("Get MythicEmpire.Cards Error!");
            if (stockCards != null)
            {
                // get player cards' rarity
                Dictionary<int, int> nRarity = new Dictionary<int, int>()
                {
                    { 4, 0 }, { 3, 0 }, { 2, 0 }, { 1, 0 }
                };
                int star = 0;
                float f_star = 0;
                foreach (var rivalCard in rivalCards)
                {
                    var stockCard = stockCards.FirstOrDefault(c => c.CardId == rivalCard);
                    if (stockCard != null)
                    {
                        var rarity = stockCard.CardRarity;
                        if (nRarity.TryGetValue(rarity, out int n))
                        {
                            n++;
                        }
                        f_star += stockCard.CardStar / 8f;
                    }
                }
                star = (int)Math.Round(f_star);
                //// set rarity for cards
                {
                    int i, j;
                    (CardType, string) temp;
                    bool swapped;
                    // sort card selecting list by priority (defend on bot play mode)
                    for (i = 0; i < cardSelected.Count - 1; i++)
                    {
                        swapped = false;
                        for (j = 0; j < cardSelected.Count - i - 1; j++)
                        {
                            if (!SelectCardService.IsGreaterThan(cardSelected[j], cardSelected[j + 1], playMode))
                            {
                                temp = cardSelected[j];
                                cardSelected[j] = cardSelected[j + 1];
                                cardSelected[j + 1] = temp;
                                swapped = true;
                            }
                        }
                        if (swapped == false)
                            break;
                    }
                }
                // set rarity for cards, the more prioritized cards, the higher rarity
                List<string> cardSelectingIdList = new List<string>();
                int cardIterationIdx = 0;
                foreach (var rarity in nRarity)
                {
                    for (int i = cardIterationIdx; i < cardIterationIdx + rarity.Value; i++)
                    {
                        var stockCard = stockCards.FirstOrDefault(c =>
                            c.TypeOfCard == (int)cardSelected[i].Item1 && c.CardName == cardSelected[i].Item2
                            && c.CardRarity == rarity.Key);
                        if (stockCard != null)
                        {
                            cardSelectingIdList.Add(stockCard.CardId);
                        }
                    }
                }
                return cardSelectingIdList;
            }
            return new List<string>();
        }

        public async Task<float> CalculateEnergyRateUsing()
        {
            energyBuildTowerRate = AIConstant.energyBuildTowerRate[playMode];
            return energyBuildTowerRate;
        }
        
        public async Task<BotLogicTile[][]> CreateTowerBuildingMap(MapService map)
        {
            // get longest path
            List<Vector2Int> longestPath = map.InitLongestPath();
            // initial tower building map
            towerBuildingMapWidth = (map.Width - 3) / 2;
            towerBuildingMapHeight = map.Height;
            towerBuildingMap = new BotLogicTile[towerBuildingMapHeight][];
            for (int i = 0; i < towerBuildingMapHeight; i++)
            {
                towerBuildingMap[i] = new BotLogicTile[towerBuildingMapWidth];
            }
            // copy data from base map to tower building map
            for (int i = 0; i < towerBuildingMapHeight; i++)
            {
                for (int j = 0; j < towerBuildingMapWidth; j++)
                {
                    towerBuildingMap[j][i].Copy(map.LogicMap[j + towerBuildingMapWidth + 2][i]);
                }
            }
            // build tower building map
            for (int i = 0; i < towerBuildingMapHeight; i++)
            {
                for (int j = 0; j < towerBuildingMapWidth; j++)
                {
                    if (towerBuildingMap[j][i].TypeOfType == TypeTile.Normal && !longestPath.Contains(new Vector2Int(i, j)))
                    {
                        // count number of adjacent path tile
                        var adjacentTile = new Vector2Int[] { new Vector2Int(i - 1, j), new Vector2Int(i + 1, j), new Vector2Int(i, j - 1),
                        new Vector2Int(i, j + 1), new Vector2Int(i - 1, j - 1), new Vector2Int(i - 1, j + 1),
                        new Vector2Int(i + 1, j - 1), new Vector2Int(i + 1, j + 1)};
                        int nAdjacentTile = 0;
                        foreach (var tile in adjacentTile)
                        {
                            if (longestPath.Contains(tile))
                            {
                                nAdjacentTile++;
                            }
                        }
                        // get a random tower from tower group base on number of adjacent path tile
                        int towerGroupTier = 0;
                        if (nAdjacentTile < 2) towerGroupTier = 0;
                        else if (nAdjacentTile < 4) towerGroupTier = 1;
                        else if (nAdjacentTile < 6) towerGroupTier = 2;
                        else towerGroupTier = 3;
                        List<string> towerCanChose = FindCardSelected(AIConstant.towerTier[towerGroupTier], CardType.TowerCard);
                        towerBuildingMap[j][i].towerId = towerCanChose[new Random().Next(0, towerCanChose.Count)];
                    }
                }
            }
            return towerBuildingMap;
        }

        private List<string> FindCardSelected(List<string> idList, CardType cardType)
        {
            // Get all the card both in card selected and card tier group.
            // If having any, return them, otherwise, return all card selected.
            // This function supports to find tower for building tower building map.
            var result = new List<string>();
            var otherResult = new List<string>();
            foreach (var card in cardSelected)
            {
                if (card.Item1 == cardType)
                {
                    otherResult.Add(card.Item2);
                    if (idList.Contains(card.Item2))
                    {
                        result.Add(card.Item2);
                    }
                }
            }
            if (result.Count > 0)
            {
                return result;
            }
            return otherResult;
        }

        private async Task<bool> Battle()
        {
            behavior = new BotBT();
            return true;
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
}
