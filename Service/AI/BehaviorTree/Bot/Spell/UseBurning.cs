using Game_Realtime.Service.AI.BehaviorTree.Structure;
using System.Security.Cryptography.Xml;

namespace Game_Realtime.Service.AI.BehaviorTree.Bot.Spell
{
    public class UseBurning: Node
    {
        private BotBTData data;
        public UseBurning(ref BotBTData data)
        {
            this.data = data;
        }

        public override NodeState Evaluate()
        {
            // TODO: Use burning at position

            state = NodeState.RUNNING;
            return state;
        }
    }
}
