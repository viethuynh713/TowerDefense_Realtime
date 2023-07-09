using Game_Realtime.Model.Map;

namespace Game_Realtime.Service.AI.TowerBuildingMapService
{
    public class BotLogicTile
    {
        public int XLogicPosition;
        public int YLogicPosition;
        public TypeTile TypeOfType;
        public string towerId;

        public BotLogicTile(int x, int y)
        {
            XLogicPosition = x;
            YLogicPosition = y;
        }

        public BotLogicTile() { }

        public void SetTower(string id)
        {
            towerId = id;
        }

        public void Copy(LogicTile tile)
        {
            XLogicPosition = tile.XLogicPosition;
            YLogicPosition = tile.YLogicPosition;
            TypeOfType = tile.TypeOfType;
            towerId = "";
        }
    }
}
