using Game_Realtime.Model;
using Game_Realtime.Service.AI.BehaviorTree.Structure;
using System.Numerics;

namespace Game_Realtime.Service.AI.BehaviorTree.Bot.Spell
{
    public class CheckUseSpeedup: Node
    {
        private AiModel bot;
        private int energyRequired;
        private Vector2 enemyBasePosition;

        public CheckUseSpeedup(AiModel bot, int energyRequired, Vector2 enemyBasePosition)
        {
            this.bot = bot;
            this.energyRequired = energyRequired;
            this.enemyBasePosition = enemyBasePosition;
            Console.WriteLine("CheckUseSpeedup 1: " + enemyBasePosition.X + ", " + enemyBasePosition.Y);
        }

        public override NodeState Evaluate()
        {
            //// Check if any monster is nearer 3 tiles away from base, set position to use explore
            // check if enough energy to use
            if (bot.EnergyToBuildTower + bot.EnergyToSummonMonster < energyRequired)
            {
                state = NodeState.FAILURE;
                return state;
            }
            // Get all enemy monsters position which is nearer 3 tiles away from base
            List<Vector2> monsterNearBasePosList = new List<Vector2>();
            foreach (var monster in bot._monsters)
            {
                if (MathF.Abs(enemyBasePosition.X - monster.Value.XLogicPosition) + MathF.Abs(enemyBasePosition.Y - monster.Value.YLogicPosition) < 3)
                {
                    Console.WriteLine("CheckUseSpeedup 2: " + ((enemyBasePosition.X - monster.Value.XLogicPosition) + (enemyBasePosition.Y - monster.Value.YLogicPosition)).ToString());
                    monsterNearBasePosList.Add(new Vector2(monster.Value.XLogicPosition, monster.Value.YLogicPosition));
                }
            }
            // Use spell at the center of the monsters found
            if (monsterNearBasePosList.Count > 0)
            {
                bot.SpellUsingPosition = new Vector2(0, 0);
                foreach (var pos in monsterNearBasePosList)
                {
                    bot.SpellUsingPosition += pos / monsterNearBasePosList.Count;
                }
                bot.SpellUsingName = "Speed";
                state = NodeState.SUCCESS;
                return state;
            }

            state = NodeState.FAILURE;
            return state;
        }
    }
}
