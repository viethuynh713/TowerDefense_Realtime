using Service.Models;

namespace Game_Realtime.Service.AI
{
    public static class AIConstant
    {
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
                    { "weak", new string[] { "Crossbow", "Cannon" } },
                    { "medium", new string[] { "Fireball", "Fire", "Energy" } },
                    { "heavy", new string[] { "Gatling", "Rocket" } }
                }
            },
            {
                CardType.MonsterCard, new Dictionary<string, string[]>()
                {
                    { "weak", new string[] { "Dog", "Hero", "Slime" } },
                    { "medium", new string[] { "Legion", "Golem" } },
                    { "heavy", new string[] { "Cyber", "Wizard" } }
                }
            },
            {
                CardType.SpellCard, new Dictionary<string, string[]>()
                {
                    { "weak", new string[] { "Explode", "Heal" } },
                    { "medium", new string[] { "Toxic", "Freeze" } },
                    { "heavy", new string[] { "Speed" } }
                }
            }
        };

        public static readonly List<List<string>> TowerTier = new List<List<string>>()
        {
            new List<string> {"Energy"},                               // tier D
            new List<string> {"Crossbow", "Cannon", "Fireball"},    // tier C
            new List<string> { "Gatling", "Rocket"},              // tier B
            new List<string> { "Fire" }                             // tier A
        };

        public static readonly List<List<string>> AttackTowerTier = new List<List<string>>()
        {
            new List<string> {"Crossbow", "Cannon", "Fireball"},    // tier C
            new List<string> { "Gatling", "Rocket"},              // tier B
            new List<string> { "Fire" }                             // tier A
        };

        public static readonly Dictionary<BotPlayMode, float> EnergyBuildTowerRate = new Dictionary<BotPlayMode, float>()
        {
            { BotPlayMode.ATTACK, 0.4f },
            { BotPlayMode.DEFEND, 1f },
            { BotPlayMode.HYBRIC, 0.8f }
        };

        public static readonly Dictionary<string, List<string>> supportMonster = new Dictionary<string, List<string>>()
        {
            { "Dog", new List<string> { "Wizard", "Cyber" } },
            {"Hero", new List<string> { "Wizard", "Cyber" } },
            {"Legion", new List<string> {"Cyber", "Wizard" } },
            {"Cyber", new List<string> { "Golem", "Legion", "Dog", "Hero", "Slime" } },
            {"Golem", new List<string> { "Cyber", "Wizard" } },
            {"Wizard", new List<string> { "Legion", "Golem", "Dog", "Hero", "Slime" } },
            {"Slime", new List < string > { "Wizard", "Cyber" } }
        };

        public static readonly List<string> towerStrength = new List<string>()
        {
            "Crossbow", "Cannon", "Fireball", "Energy", "Gatling", "Rocket", "Fire"
        };
    }
}