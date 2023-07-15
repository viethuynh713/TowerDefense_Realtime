using Game_Realtime.Model.InGame;

namespace Game_Realtime.Service.AI
{
    public static class AIConstant
    {
        public static readonly string basicTowerId = "1";

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
                    { "weak", new string[] { "1", "2" } },
                    { "medium", new string[] { "3", "6", "7" } },
                    { "strong", new string[] { "4", "5" } }
                }
            },
            {
                CardType.MonsterCard, new Dictionary<string, string[]>()
                {
                    { "weak", new string[] { "1", "2", "6" } },
                    { "medium", new string[] { "3", "5" } },
                    { "strong", new string[] { "4", "7" } }
                }
            },
            {
                CardType.SpellCard, new Dictionary<string, string[]>()
                {
                    { "weak", new string[] { "2", "4" } },
                    { "medium", new string[] { "1", "3" } },
                    { "strong", new string[] { "5" } }
                }
            }
        };

        public static readonly List<List<string>> TowerTier = new List<List<string>>()
        {
            new List<string> {"7"},             // tier 1
            new List<string> {"1", "2", "3"},   // tier 2
            new List<string> {"4", "5"},        // tier 3
            new List<string> {"6"}              // tier 4
        };

        public static readonly Dictionary<BotPlayMode, float> EnergyBuildTowerRate = new Dictionary<BotPlayMode, float>()
        {
            { BotPlayMode.ATTACK, 0.4f },
            { BotPlayMode.DEFEND, 1f },
            { BotPlayMode.HYBRIC, 0.8f }
        };

        public static readonly Dictionary<string, List<string>> supportMonster = new Dictionary<string, List<string>>()
        {
            { "1", new List<string> { "7", "4" } },
            {"2", new List<string> { "7", "4" } },
            {"3", new List<string> {"4", "7" } },
            {"4", new List<string> { "5", "3", "1", "2", "6" } },
            {"5", new List<string> { "4", "7" } },
            {"7", new List<string> { "3", "5", "1", "2", "6" } },
            {"6", new List < string > { "7", "4" } }
        };

        public static readonly List<string> towerStrength = new List<string>()
        {
            "1", "2", "3", "7", "4", "5", "6"
        };
    }
}