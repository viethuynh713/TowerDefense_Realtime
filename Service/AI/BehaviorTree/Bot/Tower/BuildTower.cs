using Service.Models;
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
                    Console.WriteLine("Prepare to sell tower");
                    string sellTowerId = "";
                    foreach (var tower in bot._towers)
                    {
                        if (tower.Value.XLogicPosition == bot.TowerSelectPos.Value.x && tower.Value.YLogicPosition == bot.TowerSelectPos.Value.y)
                        {
                            Console.WriteLine("Sell tower");
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
            // send request building tower
            var c = AIMethod.GetCardModel(bot.CardSelected, (CardType.TowerCard, towerName));
            CardModel card = new CardModel();
            if (c != null)
            {
                card = c;
            }
            Console.WriteLine("Bot build tower " + card.CardId + " at (" +
                bot.TowerSelectPos.Value.x.ToString() + ", " + bot.TowerSelectPos.Value.y.ToString() + ")");
            bot.GameSessionModel.BuildTower(bot.userId, new Model.Data.BuildTowerData()
            {
                cardId = card.CardId,
                Xposition = bot.TowerSelectPos.Value.x + bot.TowerBuildingMapWidth + 2,
                Yposition = bot.TowerSelectPos.Value.y + 1,
                stats = new Model.Data.TowerStats
                {
                    Energy = card.Energy
                }
            });
            // cost energy
            bot.EnergyToBuildTower -= card.Energy;

            state = NodeState.RUNNING;
            return state;
        }
    }
}
