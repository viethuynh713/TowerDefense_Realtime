using Game_Realtime.Model;
using Game_Realtime.Service.AI.BehaviorTree.Structure;

namespace Game_Realtime.Service.AI.BehaviorTree.Bot.Tower
{
    public class CheckBuildTower: Node
    {
        private AiModel bot;

        public CheckBuildTower(AiModel bot)
        {
            this.bot = bot;
        }

        public override NodeState Evaluate()
        {
            // check if a tower is selected to build
            if (bot.TowerSelectPos == null)
            {
                state = NodeState.FAILURE;
                return state;
            }
            // check if enough energy to use
            int energy = 1; // energy[bot.TowerBuildingMap[bot.TowerSelectPos.Value.y][bot.TowerSelectPos.Value.x].towerId]
            if (bot.EnergyToBuildTower <= energy)
            {
                state = NodeState.SUCCESS;
                return state;
            }

            state = NodeState.FAILURE;
            return state;
        }
    }
}
