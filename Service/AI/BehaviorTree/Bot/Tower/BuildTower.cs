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
            var tile = bot.TowerBuildingMap[bot.TowerSelectPos.Value.y][bot.TowerSelectPos.Value.x];
            // get tower id from selected tile
            string towerName = tile.towerName;
            if (bot.FindTowerTypeStrategy == FindTowerTypeStrategy.PROGRESS)
            {
                // if find tower building type is PROGRESS and the tower will be build on a basic tower, sell basic tower before
                if (tile.isInProgress)
                {
                    string sellTowerId = "";
                    foreach (var tower in bot._towers)
                    {
                        if (tower.Value.XLogicPosition == bot.TowerSelectPos.Value.x && tower.Value.YLogicPosition == bot.TowerSelectPos.Value.y)
                        {
                            sellTowerId = tower.Key;
                            break;
                        }
                    }
                    bot.GameSessionModel.SellTower(bot.userId, new Model.Data.SellTowerData()
                    {
                        towerId = sellTowerId
                    });
                    bot.TowerBuildingMap[bot.TowerSelectPos.Value.y][bot.TowerSelectPos.Value.x].isInProgress = false;
                    bot.TowerBuildingMap[bot.TowerSelectPos.Value.y][bot.TowerSelectPos.Value.x].hasTower = true;
                }
                else
                {
                    if (towerName == bot.BasicTowerName)
                    {
                        bot.TowerBuildingMap[bot.TowerSelectPos.Value.y][bot.TowerSelectPos.Value.x].hasTower = true;
                    }
                    // if find tower building type is PROGRESS, the tower is not a basic tower and tile is empty, build a basic tower
                    else
                    {
                        towerName = bot.BasicTowerName;
                        bot.TowerBuildingMap[bot.TowerSelectPos.Value.y][bot.TowerSelectPos.Value.x].isInProgress = true;
                    }
                }
            }
            else
            {
                bot.TowerBuildingMap[bot.TowerSelectPos.Value.y][bot.TowerSelectPos.Value.x].hasTower = true;
            }
            Console.WriteLine("Bot build tower " + AIMethod.GetCardId(bot.CardSelected, (CardType.TowerCard, towerName)) + " at (" +
                bot.TowerSelectPos.Value.x.ToString() + ", " + bot.TowerSelectPos.Value.y.ToString() + ")");
            // send request building tower
            bot.GameSessionModel.BuildTower(bot.userId, new Model.Data.BuildTowerData()
            {
                cardId = AIMethod.GetCardId(bot.CardSelected, (CardType.TowerCard, towerName)),
                Xposition = bot.TowerSelectPos.Value.x + bot.TowerBuildingMapWidth + 2,
                Yposition = bot.TowerSelectPos.Value.y + 1,
                stats = new Model.Data.TowerStats()
            });
            // cost energy
            bot.EnergyToBuildTower -= AIMethod.GetEnergy(bot.CardSelected, (CardType.TowerCard, towerName));

            state = NodeState.RUNNING;
            return state;
        }
    }
}
