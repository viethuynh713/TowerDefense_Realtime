using Game_Realtime.Model;
using Game_Realtime.Service.AI.BehaviorTree.Structure;
using System.Security.Cryptography.Xml;
using System.Threading;

namespace Game_Realtime.Service.AI.BehaviorTree.Bot.Spell
{
    public class CheckUseBurning: Node
    {
        private BotBTData data;
        private MonsterModel[] monsterList;

        public CheckUseBurning(ref BotBTData data, MonsterModel[] monsterList)
        {
            this.data = data;
            this.monsterList = monsterList;
        }

        public override NodeState Evaluate()
        {
            for (int i = 0; i < data.towerBuildingMapHeight; i++)
            {
                for (int j = 0; j < data.towerBuildingMapWidth; j++)
                {
                    // TODO: Check if any tile has 5 monsters or more nearby, set position to use burning
                    state = NodeState.SUCCESS;
                    return state;
                }
            }

            state = NodeState.FAILURE;
            return state;
        }
    }
}
