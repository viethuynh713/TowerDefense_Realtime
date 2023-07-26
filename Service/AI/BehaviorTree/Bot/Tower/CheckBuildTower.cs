using Game_Realtime.Model;
using Service.Models;
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
            string towerId = bot.TowerBuildingMap[bot.TowerSelectPos.Value.y][bot.TowerSelectPos.Value.x].towerName;
            var c = AIMethod.GetCardModel(bot.CardSelected, (CardType.TowerCard, towerId));
            CardModel card = new CardModel();
            if (c != null)
            {
                card = c;
            }
            if (bot.EnergyToBuildTower >= card.Energy)
            {
                state = NodeState.SUCCESS;
                return state;
            }

            state = NodeState.FAILURE;
            return state;
        }
    }
}
