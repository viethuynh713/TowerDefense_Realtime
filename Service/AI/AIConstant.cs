using Game_Realtime.Model.InGame;

namespace Game_Realtime.Service.AI
{
    public static class AIConstant
    {
        public static readonly string basicTowerName = "ArrowTower";

        public static readonly Dictionary<BotPlayMode, Dictionary<CardType, Dictionary<string, int>>> CardSelectingStrategy = new Dictionary<BotPlayMode, Dictionary<CardType, Dictionary<string, int>>>()
        {
            { BotPlayMode.ATTACK, new Dictionary<CardType, Dictionary<string, int>>()
                {
                    { CardType.TowerCard, new Dictionary<string, int>() { { "weak", 1 }, { "medium", 0 }, { "heavy", 0 } } },
                    { CardType.MonsterCard, new Dictionary<string, int>() { { "weak", 2 }, { "medium", 1 }, { "heavy", 1 } } },
                    { CardType.SpellCard, new Dictionary<string, int>() { { "weak", 2 }, { "medium", 0 }, { "heavy", 1 } } }
                }
            },
            { BotPlayMode.DEFEND, new Dictionary<CardType, Dictionary<string, int>>()
                {
                    { CardType.TowerCard, new Dictionary<string, int>() { { "weak", 2 }, { "medium", 2 }, { "heavy", 2 } } },
                    { CardType.MonsterCard, new Dictionary<string, int>() { { "weak", 1 }, { "medium", 0 }, { "heavy", 0 } } },
                    { CardType.SpellCard, new Dictionary<string, int>() { { "weak", 1 }, { "medium", 0 }, { "heavy", 0 } } }
                }
            },
            { BotPlayMode.HYBRIC, new Dictionary<CardType, Dictionary<string, int>>()
                {
                    { CardType.TowerCard, new Dictionary<string, int>() { { "weak", 2 }, { "medium", 1 }, { "heavy", 1 } } },
                    { CardType.MonsterCard, new Dictionary<string, int>() { { "weak", 1 }, { "medium", 0 }, { "heavy", 1 } } },
                    { CardType.SpellCard, new Dictionary<string, int>() { { "weak", 1 }, { "medium", 0 }, { "heavy", 1 } } }
                }
            }
        };

        public static readonly Dictionary<CardType, Dictionary<string, string[]>> CardGroup = new Dictionary<CardType, Dictionary<string, string[]>>()
        {
            {
                CardType.TowerCard, new Dictionary<string, string[]>()
                {
                    { "weak", new string[] { "ArrowTower", "CannonTower" } },
                    { "medium", new string[] { "RockTower", "Flamethrower", "EnergyTower" } },
                    { "strong", new string[] { "GatlingTower", "RocketTower" } }
                }
            },
            {
                CardType.MonsterCard, new Dictionary<string, string[]>()
                {
                    { "weak", new string[] { "DogPolyart", "FemaleCharacter", "Slime" } },
                    { "medium", new string[] { "Footman", "Golem" } },
                    { "strong", new string[] { "SciFiWarrior", "Wizard" } }
                }
            },
            {
                CardType.SpellCard, new Dictionary<string, string[]>()
                {
                    { "weak", new string[] { "Explore", "Healing" } },
                    { "medium", new string[] { "Burning", "Freeze" } },
                    { "strong", new string[] { "Speedup" } }
                }
            }
        };

        public static readonly List<List<string>> TowerTier = new List<List<string>>()
        {
            new List<string> {"EnergyTower"},                               // tier D
            new List<string> {"ArrowTower", "CannonTower", "RockTower"},    // tier C
            new List<string> { "GatlingTower", "RocketTower"},              // tier B
            new List<string> { "Flamethrower" }                             // tier A
        };

        public static readonly Dictionary<BotPlayMode, float> EnergyBuildTowerRate = new Dictionary<BotPlayMode, float>()
        {
            { BotPlayMode.ATTACK, 0.4f },
            { BotPlayMode.DEFEND, 1f },
            { BotPlayMode.HYBRIC, 0.8f }
        };

        public static readonly Dictionary<string, List<string>> supportMonster = new Dictionary<string, List<string>>()
        {
            { "DogPolyart", new List<string> { "Wizard", "SciFiWarrior" } },
            {"FemaleCharacter", new List<string> { "Wizard", "SciFiWarrior" } },
            {"Footman", new List<string> {"SciFiWarrior", "Wizard" } },
            {"SciFiWarrior", new List<string> { "Golem", "Footman", "DogPolyart", "FemaleCharacter", "Slime" } },
            {"Golem", new List<string> { "SciFiWarrior", "Wizard" } },
            {"Wizard", new List<string> { "Footman", "Golem", "DogPolyart", "FemaleCharacter", "Slime" } },
            {"Slime", new List < string > { "Wizard", "SciFiWarrior" } }
        };

        public static readonly List<string> towerStrength = new List<string>()
        {
            "ArrowTower", "CannonTower", "RockTower", "EnergyTower", "GatlingTower", "RocketTower", "Flamethrower"
        };
    }
}