using Game_Realtime.Model.InGame;
using Game_Realtime.Model;
using Game_Realtime.Service.AI.BehaviorTree.Structure;

namespace Game_Realtime.Service.AI.BehaviorTree.Bot.Tower
{
    public class BuildTower: Node
    {
        private AiModel bot;

        public BuildTower(AiModel bot)
        {
            this.bot = bot;
        }

        public override NodeState Evaluate()
        {
            // get tower id from selected tile
            string towerId = bot.TowerBuildingMap[bot.TowerSelectPos.Value.y][bot.TowerSelectPos.Value.x].towerId;
            // if find tower building type is PROGRESS, the tower is not a basic tower and tile is empty, build a basic tower
            if (bot.FindTowerTypeStrategy == FindTowerTypeStrategy.PROGRESS && towerId != AIConstant.basicTowerId
                && !bot.TowerBuildingMap[bot.TowerSelectPos.Value.y][bot.TowerSelectPos.Value.x].isInProgress)
            {
                towerId = AIConstant.basicTowerId;
                bot.TowerBuildingMap[bot.TowerSelectPos.Value.y][bot.TowerSelectPos.Value.x].isInProgress = true;
            }
            // send request building tower
            bot.BuildTower(new Model.Data.BuildTowerData()
            {
                cardId = towerId,
                Xposition = bot.TowerSelectPos.Value.x,
                Yposition = bot.TowerSelectPos.Value.y,
                stats = new Model.Data.TowerStats()
            });

            state = NodeState.RUNNING;
            return state;
        }
    }
}
