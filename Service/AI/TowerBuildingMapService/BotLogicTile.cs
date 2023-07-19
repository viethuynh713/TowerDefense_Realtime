using Game_Realtime.Model.Map;

namespace Game_Realtime.Service.AI.TowerBuildingMapService
{
    public class BotLogicTile
    {
        public int XLogicPosition;
        public int YLogicPosition;
        public TypeTile TypeOfType;
        public string towerName;
        public bool isBuildTower;       // will be used to build tower or not
        public bool? hasTower;          // have tower or not
        public bool isInProgress;       // is being build with the basic tower? (only use if bot building tower type is PROGRESS)
        public int damageLevel;
        public int fireRateLevel;
        public int rangeLevel;

        public BotLogicTile(int x, int y)
        {
            XLogicPosition = x;
            YLogicPosition = y;
        }

        public BotLogicTile() { }

        public void SetTower(string id)
        {
            towerName = id;
        }

        public void Copy(LogicTile tile)
        {
            XLogicPosition = tile.XLogicPosition;
            YLogicPosition = tile.YLogicPosition;
            TypeOfType = tile.TypeOfType;
            towerName = "";
            isBuildTower = false;
            hasTower = null;
            isInProgress = false;
            damageLevel = 0;
            fireRateLevel = 0;
            rangeLevel = 0;
        }
    }
}
