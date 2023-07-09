using Game_Realtime.Model.InGame;
using Game_Realtime.Service.AI.BehaviorTree.Structure;
using Game_Realtime.Service.AI.TowerBuildingMapService;

namespace Game_Realtime.Service.AI.BehaviorTree.Bot
{
    public class BotBT: Tree
    {
        BotBTData data = new BotBTData();
        protected override Node SetUpTree()
        {
            Node spellTree = new Selector(new List<Node>
            {
                new Sequence()
            });

            Node root = new Sequence(new List<Node>
            {
                new DivideEnergy(ref data),
                spellTree
            });

            return root;
        }

        public void SetData(BotPlayMode playMode, List<(CardType, string)> cardSelected, BotLogicTile[][] towerBuildingMap, float energyBuildTowerRate)
        {
            data.playMode = playMode;
            data.cardSelected = cardSelected;
            data.towerBuildingMap = towerBuildingMap;
            data.towerBuildingMapWidth = towerBuildingMap[0].Length;
            data.towerBuildingMapHeight = towerBuildingMap.Length;
            data.energyBuildTowerRate = energyBuildTowerRate;
        }
    }
}
