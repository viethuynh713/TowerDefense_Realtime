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
                        new CheckSpawnMonsterToDefend(bot, new Vector2(0, 4)),
                        new SpawnMonsterToDefend(bot, new Vector2(0, 4))
                    }),
                    new Sequence(new List<Node>
                    {
                        new CheckSpawnMonsterToAttack(bot),
                        new SpawnMonsterToAttack(bot, new Vector2Int(10, 4))
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
                    int cardEnergy = AIMethod.GetEnergy(bot.CardSelected, (CardType.SpellCard, "Toxic"));
                    spellNodeList.Add(new Sequence(new List<Node>()
                    {
                        new CheckUseBurning(bot, cardEnergy),
                        new UseBurning(bot, cardEnergy)
                    }));
                }
            }
            if (spellName == "Explode")
            {
                if (AIMethod.IsBotCardSelectedContain(bot.CardSelected, (CardType.SpellCard, "Explode")))
                {
                    int cardEnergy = AIMethod.GetEnergy(bot.CardSelected, (CardType.SpellCard, "Explode"));
                    spellNodeList.Add(new Sequence(new List<Node>()
                    {
                        new CheckUseExplore(bot, cardEnergy, new Vector2(0, 4)),
                        new UseExplore(bot, cardEnergy)
                    }));
                }
            }
            if (spellName == "Freeze")
            {
                if (AIMethod.IsBotCardSelectedContain(bot.CardSelected, (CardType.SpellCard, "Freeze")))
                {
                    int cardEnergy = AIMethod.GetEnergy(bot.CardSelected, (CardType.SpellCard, "Freeze"));
                    spellNodeList.Add(new Sequence(new List<Node>() {
                        new CheckUseFreeze(bot, cardEnergy),
                        new UseFreeze(bot, cardEnergy)
                    })); ;
                }
            }
            if (spellName == "Heal")
            {
                if (AIMethod.IsBotCardSelectedContain(bot.CardSelected, (CardType.SpellCard, "Heal")))
                {
                    int cardEnergy = AIMethod.GetEnergy(bot.CardSelected, (CardType.SpellCard, "Heal"));
                    spellNodeList.Add(new Sequence(new List<Node>() {
                        new CheckUseHealing(bot, cardEnergy),
                        new UseHealing(bot, cardEnergy)
                    })); 
                }
            }
            if (spellName == "Speed")
            {
                if (AIMethod.IsBotCardSelectedContain(bot.CardSelected, (CardType.SpellCard, "Speed")))
                {
                    int cardEnergy = AIMethod.GetEnergy(bot.CardSelected, (CardType.SpellCard, "Speed"));
                    spellNodeList.Add(new Sequence(new List<Node>()
                    {
                        new CheckUseSpeedup(bot, cardEnergy, new Vector2(20, 4)),
                        new UseSpeedup(bot, cardEnergy)
                    }));
                }
            }
        }
    }
}
