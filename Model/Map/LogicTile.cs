namespace Game_Realtime.Model.Map
{
    public class LogicTile
    {
        public string OwnerId;
        public int XLogicPosition;
        public int YLogicPosition;
        public TypeTile TypeOfType;

        public LogicTile(int x, int y)
        {
            XLogicPosition = x;
            YLogicPosition = y;
        }

        public LogicTile(){}

    }

    public enum TypeTile
    {
        Normal,
        Castle,
        Gate,
        Barrier,
        Bridge
    }
}
