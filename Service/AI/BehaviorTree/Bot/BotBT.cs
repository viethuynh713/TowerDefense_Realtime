using Service.Models;
using Game_Realtime.Service.AI.BehaviorTree.Structure;
using Game_Realtime.Service.AI.BehaviorTree.Bot.Spell;
using Game_Realtime.Model;
using Game_Realtime.Service.AI.BehaviorTree.Bot.Monster;
using Game_Realtime.Service.AI.BehaviorTree.Bot.Tower;

namespace Game_Realtime.Service.AI.BehaviorTree.Bot
{
    public class BotBT: Tree
    {
        AiModel bot;

        public BotBT(AiModel bot)
        {
            this.bot = bot;
            bot.TowerSelectPos = bot.TowerBuildOrder[0];
            bot.TowerBuildOrder.RemoveAt(0);
            root = SetUpTree();
        }

        protected override Node SetUpTree()
        {
            List<Node> spellNodeList = new List<Node>();
            switch (bot.PlayMode)
            {
                case BotPlayMode.ATTACK:
                    AddSpellBehavior(ref spellNodeList, "Speed");
                    AddSpellBehavior(ref spellNodeList, "Heal");
                    AddSpellBehavior(ref spellNodeList, "Toxic");
                    AddSpellBehavior(ref spellNodeList, "Explode");
                    break;
                case BotPlayMode.DEFEND:
                    AddSpellBehavior(ref spellNodeList, "Explode");
                    AddSpellBehavior(ref spellNodeList, "Freeze");
                    AddSpellBehavior(ref spellNodeList, "Toxic");
                    AddSpellBehavior(ref spellNodeList, "Heal");
                    AddSpellBehavior(ref spellNodeList, "Speed");
                    break;
                case BotPlayMode.HYBRIC:
                    AddSpellBehavior(ref spellNodeList, "Toxic");
                    AddSpellBehavior(ref spellNodeList, "Heal");
                    AddSpellBehavior(ref spellNodeList, "Explode");
                    AddSpellBehavior(ref spellNodeList, "Speed");
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
                        new CheckSpawnMonsterToDefend(bot, bot.GameSessionModel._mapService._castleLogicPosition[bot.userId].CastToVector2()),
                        new SpawnMonsterToDefend(bot, bot.GameSessionModel._mapService._castleLogicPosition[bot.userId].CastToVector2())
                    }),
                    new Sequence(new List<Node>
                    {
                        new CheckSpawnMonsterToAttack(bot),
                        new SpawnMonsterToAttack(bot, bot.GameSessionModel._mapService.MonsterGate)
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

        private void AddSpellBehavior(ref List<Node> spellNodeList, string spellName)
        {
            if (spellName == "Toxic")
            {
                if (AIMethod.IsBotCardSelectedContain(bot.CardSelected, (CardType.SpellCard, "Toxic")))
                {
                    spellNodeList.Add(new Sequence(new List<Node>()
                    {
                        new CheckUseBurning(bot),
                        new UseBurning(bot)
                    }));
                }
            }
            if (spellName == "Explode")
            {
                if (AIMethod.IsBotCardSelectedContain(bot.CardSelected, (CardType.SpellCard, "Explode")))
                {
                    spellNodeList.Add(new Sequence(new List<Node>()
                    {
                        new CheckUseExplore(bot, bot.GameSessionModel._mapService._castleLogicPosition[bot.userId].CastToVector2()),
                        new UseExplore(bot)
                    }));
                }
            }
            if (spellName == "Freeze")
            {
                if (AIMethod.IsBotCardSelectedContain(bot.CardSelected, (CardType.SpellCard, "Freeze")))
                {
                    spellNodeList.Add(new Sequence(new List<Node>() {
                        new CheckUseFreeze(bot),
                        new UseFreeze(bot)
                    })); ;
                }
            }
            if (spellName == "Heal")
            {
                if (AIMethod.IsBotCardSelectedContain(bot.CardSelected, (CardType.SpellCard, "Heal")))
                {
                    spellNodeList.Add(new Sequence(new List<Node>() {
                        new CheckUseHealing(bot),
                        new UseHealing(bot)
                    })); 
                }
            }
            if (spellName == "Speed")
            {
                if (AIMethod.IsBotCardSelectedContain(bot.CardSelected, (CardType.SpellCard, "Speed")))
                {
                    spellNodeList.Add(new Sequence(new List<Node>()
                    {
                        new CheckUseSpeedup(bot, bot.GameSessionModel._mapService._castleLogicPosition[bot.GameSessionModel.GetRivalPlayer(bot.userId).userId].CastToVector2()),
                        new UseSpeedup(bot)
                    }));
                }
            }
        }
    }
}
