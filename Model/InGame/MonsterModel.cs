using System.Numerics;

namespace Game_Realtime.Model
{
    public class MonsterModel
    {
        public string monsterId;
        public string cardId;
        public int monsterHp;
        public Vector2 monsterPosition;

        public MonsterModel(string cardId, int monsterHp,Vector2 monsterPosition)
        {
            this.monsterId = Guid.NewGuid().ToString();
            this.cardId = cardId;
            this.monsterHp = monsterHp;
            this.monsterPosition = monsterPosition;
        }
    }
}
