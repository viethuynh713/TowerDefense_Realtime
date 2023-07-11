using Game_Realtime.Model;
using Game_Realtime.Service.AI.BehaviorTree.Structure;
using System.Numerics;

namespace Game_Realtime.Service.AI.BehaviorTree.Bot.Spell
{
    public class CheckUseExplore: Node
    {
        private BotBTData data;
        private int energyRequired;
        private string playerId;
        private MonsterModel[] monsterList;
        private Vector2 botBasePosition;

        public CheckUseExplore(ref BotBTData data, int energyRequired, string playerId, MonsterModel[] monsterList, Vector2 botBasePosition)
        {
            this.data = data;
            this.energyRequired = energyRequired;
            this.playerId = playerId;
            this.monsterList = monsterList;
            this.botBasePosition = botBasePosition;
        }

        public override NodeState Evaluate()
        {
            //// Check if any monster is nearer 3 tiles away from base, set position to use explore
            // check if enough energy to use
            if (data.energyToBuildTower + data.energyToSummonMonster < energyRequired)
            {
                state = NodeState.FAILURE;
                return state;
            }
            // Get all enemy monsters position which is nearer 3 tiles away from base
            List<Vector2> monsterNearBasePosList = new List<Vector2>();
            foreach (var monster in monsterList)
            {
                if (monster.ownerId == playerId)
                {
                    if (MathF.Sqrt(MathF.Pow(botBasePosition.X - monster.XLogicPosition, 2) + MathF.Pow(botBasePosition.Y - monster.YLogicPosition, 2)) <= 3)
                    {
                        monsterNearBasePosList.Add(new Vector2(monster.XLogicPosition, monster.YLogicPosition));
                    }
                }
            }
            // Use spell at the center of the monsters found
            if (monsterNearBasePosList.Count > 0)
            {
                data.spellUsingPosition = new Vector2(0, 0);
                foreach (var pos in monsterNearBasePosList)
                {
                    data.spellUsingPosition += pos / monsterNearBasePosList.Count;
                }
                data.spellUsingName = "Explore";
                state = NodeState.SUCCESS;
                return state;
            }

            state = NodeState.FAILURE;
            return state;
        }
    }
}
