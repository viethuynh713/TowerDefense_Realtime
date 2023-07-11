using Game_Realtime.Service.AI.BehaviorTree.Structure;

namespace Game_Realtime.Service.AI.BehaviorTree.Bot.Spell
{
    public class UseSpeedup : Node
    {
        private BotBTData data;
        public UseSpeedup(ref BotBTData data)
        {
            this.data = data;
        }

        public override NodeState Evaluate()
        {
            // TODO: Use burning at position
            // ActiveSpell(data.spellUsingName, data.spellUsingPosition);

            state = NodeState.RUNNING;
            return state;
        }
    }
}
