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
            Console.WriteLine("Check Build Tower Map Complete");
            // check if map build tower completely (not check level up)
            bool isBuildTowerComplete = true;
            foreach (var row in bot.TowerBuildingMap)
            {
                foreach (var tile in row)
                {
                    if (tile.hasTower != null && tile.isBuildTower)
                    {
                        if (!tile.hasTower.Value || tile.isInProgress)
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
