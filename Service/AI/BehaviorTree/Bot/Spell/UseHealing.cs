using Game_Realtime.Service.AI.BehaviorTree.Structure;

namespace Game_Realtime.Service.AI.BehaviorTree.Bot.Spell
{
    public class UseHealing : Node
    {
        private BotBTData data;
        public UseHealing(ref BotBTData data)
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
