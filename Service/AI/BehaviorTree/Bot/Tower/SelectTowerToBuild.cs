using Game_Realtime.Model;
using Game_Realtime.Service.AI.BehaviorTree.Structure;

namespace Game_Realtime.Service.AI.BehaviorTree.Bot.Tower
{
    public class SelectTowerToBuild : Node
    {
        private AiModel bot;

        public SelectTowerToBuild(AiModel bot)
        {
            this.bot = bot;
        }

        public override NodeState Evaluate()
        {
            // choose the first position in order
            if (bot.TowerBuildOrder.Count > 0)
            {
                bot.TowerSelectPos = bot.TowerBuildOrder[0];
                bot.TowerBuildOrder.RemoveAt(0);
            }
            // otherwise, if find tower building type is PROGRESS, choose the first position in progress order
            else if (bot.TowerBuildProgressOrder.Count > 0)
            {
                bot.TowerSelectPos = bot.TowerBuildProgressOrder[0];
                bot.TowerBuildProgressOrder.RemoveAt(0);
            }

            state = NodeState.RUNNING;
            return state;
        }
    }
}
