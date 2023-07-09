using Game_Realtime.Model.InGame;
using Game_Realtime.Service.AI.TowerBuildingMapService;
using System.Numerics;

namespace Game_Realtime.Service.AI.BehaviorTree.Bot
{
    public class BotBTData
    {
        public BotPlayMode playMode;
        public List<(CardType, string)> cardSelected;
        public BotLogicTile[][] towerBuildingMap;
        public int towerBuildingMapWidth;
        public int towerBuildingMapHeight;
        public float energyBuildTowerRate;

        public float energyToBuildTower = 0;
        public float energyToSummonMonster = 0;
        public int energyGain = 0;

        public Vector2 spellUsingPosition = new Vector2();
    }
}
