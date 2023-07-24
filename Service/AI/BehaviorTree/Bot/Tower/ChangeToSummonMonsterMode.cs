using Game_Realtime.Model;
using Game_Realtime.Service.AI.BehaviorTree.Structure;

namespace Game_Realtime.Service.AI.BehaviorTree.Bot.Tower
{
    public class ChangeToSummonMonsterMode: Node
    {
        private AiModel bot;

        public ChangeToSummonMonsterMode(AiModel bot)
        {
            this.bot = bot;
        }

        public override NodeState Evaluate()
        {
            Console.WriteLine("Change To Summon Monster Mode");
            bot.EnergyBuildTowerRate = 0;

            state = NodeState.RUNNING;
            return state;
        }
    }
}
