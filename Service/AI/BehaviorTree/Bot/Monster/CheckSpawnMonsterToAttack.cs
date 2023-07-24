using Game_Realtime.Model;
using Game_Realtime.Service.AI.BehaviorTree.Structure;
using System.Numerics;

namespace Game_Realtime.Service.AI.BehaviorTree.Bot.Monster
{
    public class CheckSpawnMonsterToAttack: Node
    {
        private AiModel bot;
        private int minEnergyRequired;

        public CheckSpawnMonsterToAttack(AiModel bot)
        {
            this.bot = bot;
            minEnergyRequired = AIMethod.GetMinMonsterEnergy(bot.CardSelected);
        }

        public override NodeState Evaluate()
        {
            Console.WriteLine("Check Spawn Monster To Attack");
            // check if enough energy to use
            if (bot.EnergyToSummonMonster <= minEnergyRequired)
            {
                state = NodeState.FAILURE;
                return state;
            }
            // if new monster wave is summoned currently, summon monsters
            if (bot.HasAutoSummonMonsterCurrently)
            {
                state = NodeState.SUCCESS;
                return state;
            }

            state = NodeState.FAILURE;
            return state;
        }
    }
}
