using Game_Realtime.Model;
using Game_Realtime.Service.AI.BehaviorTree.Structure;
using System.Numerics;

namespace Game_Realtime.Service.AI.BehaviorTree.Bot.Spell
{
    public class CheckUseHealing: Node
    {
        private AiModel bot;
        private int energyRequired;
        private MonsterModel[] monsterList;

        public CheckUseHealing(AiModel bot, int energyRequired, MonsterModel[] monsterList)
        {
            this.bot = bot;
            this.energyRequired = energyRequired;
            this.monsterList = monsterList;
        }

        public override NodeState Evaluate()
        {
            //// Check if any tile has 5 monsters or more nearby and at least 3 monsters has less than 30%HP, set position to use burning
            // check if enough energy to use
            if (bot.EnergyToBuildTower + bot.EnergyToSummonMonster < energyRequired)
            {
                state = NodeState.FAILURE;
                return state;
            }
            // create a hash table with key is map tile position and value is monster nearby (in range of burning spell)
            // (int, int): the first is number of monsters, the second is number of monsters whose HP is less than 30%
            Dictionary<Vector2, (int, int)> monstersInRange = new Dictionary<Vector2, (int, int)>();
            float HPRate = 0.6f;
            foreach (var monster in monsterList)
            {
                if (monster.ownerId == bot.userId)
                {
                    for (int x = -1; x <= 1; x++)
                    {
                        for (int y = -1; y <= 1; y++)
                        {
                            if (monster.XLogicPosition + x >= 0 && monster.XLogicPosition + x < bot.TowerBuildingMapWidth
                                && monster.YLogicPosition + y >= 0 && monster.YLogicPosition + y < bot.TowerBuildingMapHeight)
                            {
                                if (monstersInRange.TryGetValue(new Vector2(monster.XLogicPosition + x, monster.YLogicPosition + y), out var value))
                                {
                                    value.Item1++;
                                    if (monster.monsterHp / monster.maxHp < HPRate)
                                    {
                                        value.Item2++;
                                    }
                                }
                                else
                                {
                                    if (monster.monsterHp / monster.maxHp < HPRate)
                                    {
                                        monstersInRange.Add(new Vector2(monster.XLogicPosition + x, monster.YLogicPosition + y), (1, 1));
                                    }
                                    else
                                    {
                                        monstersInRange.Add(new Vector2(monster.XLogicPosition + x, monster.YLogicPosition + y), (1, 0));
                                    }
                                }
                            }
                        }
                    }
                }
            }
            // get the map tile which has most monster nearby and has at least 3 monsters whose HP is less than 60%
            Vector2 crowdestPosition = new Vector2(0, 0);
            (int, int) nMonsterInCrowded = (0, 0);
            foreach (var monsters in monstersInRange)
            {
                if (monsters.Value.Item1 > nMonsterInCrowded.Item1 && monsters.Value.Item2 >= 3)
                {
                    crowdestPosition = monsters.Key;
                    nMonsterInCrowded = monsters.Value;
                }
                else if (monsters.Value.Item1 == nMonsterInCrowded.Item1)
                {
                    int rand = new Random().Next(0, 2);
                    if (monsters.Value.Item2 > nMonsterInCrowded.Item2
                        || (monsters.Value.Item2 == nMonsterInCrowded.Item2 && rand == 0))
                    {
                        crowdestPosition = monsters.Key;
                        nMonsterInCrowded = monsters.Value;
                    }
                }
            }
            // if enough monster to use spell, return spell type and position
            if (nMonsterInCrowded.Item1 >= 5)
            {
                bot.SpellUsingPosition = crowdestPosition;
                bot.SpellUsingName = "Healing";
                state = NodeState.SUCCESS;
                return state;
            }

            state = NodeState.FAILURE;
            return state;
        }
    }
}
