using Game_Realtime.Model;
using Game_Realtime.Service.AI.BehaviorTree.Structure;

namespace Game_Realtime.Service.AI.BehaviorTree.Bot.Tower
{
    public class CheckBuildTowerMapComplete: Node
    {
        private AiModel bot;

        public CheckBuildTowerMapComplete(AiModel bot)
        {
            this.bot = bot;
        }

        public override NodeState Evaluate()
        {
            // check if map build tower completely (not check level up)
            bool isBuildTowerComplete = true;
            foreach (var row in bot.TowerBuildingMap)
            {
                foreach (var tile in row)
                {
                    if (tile.hasTower != null)
                    {
                        if (tile.isBuildTower && !tile.hasTower.Value)
                        {
                            isBuildTowerComplete = false;
                            break;
                        }
                    }
                }
            }
            if (isBuildTowerComplete)
            {
                state = NodeState.FAILURE;
                return state;
            }

            state = NodeState.SUCCESS;
            return state;
        }
    }
}
