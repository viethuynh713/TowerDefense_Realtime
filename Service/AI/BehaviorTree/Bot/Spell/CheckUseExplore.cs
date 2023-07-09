using Game_Realtime.Service.AI.BehaviorTree.Structure;

namespace Game_Realtime.Service.AI.BehaviorTree.Bot.Spell
{
    public class CheckUseExplore: Node
    {
        private BotBTData data;

        public CheckUseExplore(ref BotBTData data)
        {
            this.data = data;
        }

        public override NodeState Evaluate()
        {
            // TODO: Check if any monster is nearer 3 tiles away from base
            if (true)
            {
                state = NodeState.SUCCESS;
                return state;
            }

            state = NodeState.FAILURE;
            return state;
        }
    }
}
