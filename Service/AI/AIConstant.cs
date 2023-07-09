using Game_Realtime.Model.InGame;

namespace Game_Realtime.Service.AI
{
    public static class AIConstant
    {
        public static readonly Dictionary<BotPlayMode, Dictionary<CardType, Dictionary<string, int>>> cardSelectingStrategy = new Dictionary<BotPlayMode, Dictionary<CardType, Dictionary<string, int>>>()
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

        public static readonly Dictionary<CardType, Dictionary<string, string[]>> cardGroup = new Dictionary<CardType, Dictionary<string, string[]>>()
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

        public static readonly List<List<string>> towerTier = new List<List<string>>()
        {
            new List<string> {"7"},
            new List<string> {"1", "2", "3"},
            new List<string> {"4", "5"},
            new List<string> {"6"}
        };

        public static readonly Dictionary<BotPlayMode, float> energyBuildTowerRate = new Dictionary<BotPlayMode, float>()
        {
            { BotPlayMode.ATTACK, 0.4f },
            { BotPlayMode.DEFEND, 1f },
            { BotPlayMode.HYBRIC, 0.8f }
        };
    }
}