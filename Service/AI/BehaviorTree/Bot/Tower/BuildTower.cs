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
            string towerName = bot.TowerBuildingMap[bot.TowerSelectPos.Value.y][bot.TowerSelectPos.Value.x].towerName;
            // if find tower building type is PROGRESS, the tower is not a basic tower and tile is empty, build a basic tower
            if (bot.FindTowerTypeStrategy == FindTowerTypeStrategy.PROGRESS && towerName != AIConstant.basicTowerName
                && !bot.TowerBuildingMap[bot.TowerSelectPos.Value.y][bot.TowerSelectPos.Value.x].isInProgress)
            {
                towerName = AIConstant.basicTowerName;
                bot.TowerBuildingMap[bot.TowerSelectPos.Value.y][bot.TowerSelectPos.Value.x].isInProgress = true;
            }
            // send request building tower
            bot.GameSessionModel.BuildTower(bot.userId, new Model.Data.BuildTowerData()
            {
                cardId = AIMethod.GetCardId(bot.CardSelected, (CardType.TowerCard, towerName)),
                Xposition = bot.TowerSelectPos.Value.x,
                Yposition = bot.TowerSelectPos.Value.y,
                stats = new Model.Data.TowerStats()
            });
            // cost energy
            bot.EnergyToBuildTower -= AIMethod.GetEnergy(bot.CardSelected, (CardType.TowerCard, towerName));

            state = NodeState.RUNNING;
            return state;
        }
    }
}
