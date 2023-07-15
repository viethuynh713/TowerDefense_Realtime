using Game_Realtime.Model.InGame;
using Game_Realtime.Service.AI.BehaviorTree.Structure;
using Game_Realtime.Service.AI.TowerBuildingMapService;
using Game_Realtime.Service.AI.BehaviorTree.Bot.Spell;
using Game_Realtime.Model;
using Game_Realtime.Service.AI.BehaviorTree.Bot.Monster;
using System.Numerics;
using Game_Realtime.Service.AI.BehaviorTree.Bot.Tower;

namespace Game_Realtime.Service.AI.BehaviorTree.Bot
{
    public class BotBT: Tree
    {
        AiModel bot;
        protected override Node SetUpTree()
        {
            List<Node> spellNodeList = new List<Node>();
            switch (bot.PlayMode)
            {
                case BotPlayMode.ATTACK:
                    AddSpellBehavior(ref spellNodeList, "Speedup");
                    AddSpellBehavior(ref spellNodeList, "Healing");
                    AddSpellBehavior(ref spellNodeList, "Burning");
                    AddSpellBehavior(ref spellNodeList, "Explore");
                    break;
                case BotPlayMode.DEFEND:
                    AddSpellBehavior(ref spellNodeList, "Explore");
                    AddSpellBehavior(ref spellNodeList, "Freeze");
                    AddSpellBehavior(ref spellNodeList, "Burning");
                    AddSpellBehavior(ref spellNodeList, "Healing");
                    AddSpellBehavior(ref spellNodeList, "Speedup");
                    break;
                case BotPlayMode.HYBRIC:
                    AddSpellBehavior(ref spellNodeList, "Burning");
                    AddSpellBehavior(ref spellNodeList, "Healing");
                    AddSpellBehavior(ref spellNodeList, "Explore");
                    AddSpellBehavior(ref spellNodeList, "Speedup");
                    AddSpellBehavior(ref spellNodeList, "Freeze");
                    break;
            }

            Node root = new Sequence(new List<Node>
            {
                new DivideEnergy(bot),
                new Selector(new List<Node>
                {
                    new Selector(spellNodeList),
                    new Sequence(new List<Node>
                    {
                        new CheckSpawnMonsterToDefend(bot, 0, new MonsterModel[0], new Vector2(0, 4)),
                        new SpawnMonsterToDefend(bot, new MonsterModel[0], new Vector2(0, 4))
                    }),
                    new Sequence(new List<Node>
                    {
                        new CheckSpawnMonsterToAttack(bot, 0),
                        new SpawnMonsterToAttack(bot, new Vector2Int(10, 4), new MonsterModel[0])
                    }),
                    new Sequence(new List<Node>
                    {
                        new CheckBuildTowerMapComplete(bot),
                        new Sequence(new List<Node>
                        {
                            new CheckBuildTower(bot),
                            new BuildTower(bot),
                            new Selector(new List<Node>
                            {
                                new Sequence(new List<Node>
                                {
                                    new CheckBuildTowerMapComplete(bot),
                                    new SelectTowerToBuild(bot),
                                }),
                                new ChangeToSummonMonsterMode(bot)
                            })
                        })
                    })
                })
            });

            return root;
        }

        public void SetData(AiModel bot)
        {
            this.bot = bot;
        }

        private void AddSpellBehavior(ref List<Node> spellNodeList, string spellName)
        {
            if (spellName == "Burning")
            {
                if (bot.CardSelected.Contains((CardType.SpellCard, "Burning")))
                {
                    spellNodeList.Add(new Sequence(new List<Node>()
                    {
                        new CheckUseBurning(bot, 0, new MonsterModel[0]),
                        new UseBurning(bot)
                    }));
                }
            }
            if (spellName == "Explore")
            {
                if (bot.CardSelected.Contains((CardType.SpellCard, "Explore")))
                {
                    spellNodeList.Add(new Sequence(new List<Node>()
                    {
                        new CheckUseExplore(bot, 0, new MonsterModel[0], new Vector2(0, 4)),
                        new UseExplore(bot)
                    }));
                }
            }
            if (spellName == "Freeze")
            {
                if (bot.CardSelected.Contains((CardType.SpellCard, "Freeze")))
                {
                    spellNodeList.Add(new Sequence(new List<Node>() {
                        new CheckUseFreeze(bot, 0, new MonsterModel[0]),
                        new UseFreeze(bot)
                    })); ;
                }
            }
            if (spellName == "Healing")
            {
                if (bot.CardSelected.Contains((CardType.SpellCard, "Healing")))
                {
                    spellNodeList.Add(new Sequence(new List<Node>() {
                        new CheckUseHealing(bot, 0, new MonsterModel[0]),
                        new UseHealing(bot)
                    })); 
                }
            }
            if (spellName == "Speedup")
            {
                if (bot.CardSelected.Contains((CardType.SpellCard, "Speedup")))
                {
                    spellNodeList.Add(new Sequence(new List<Node>()
                    {
                        new CheckUseSpeedup(bot, 0, new MonsterModel[0], new Vector2(20, 4)),
                        new UseSpeedup(bot)
                    }));
                }
            }
        }
    }
}
