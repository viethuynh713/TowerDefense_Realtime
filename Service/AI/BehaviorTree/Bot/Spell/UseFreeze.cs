using Game_Realtime.Model;
using Game_Realtime.Service.AI.BehaviorTree.Structure;

namespace Game_Realtime.Service.AI.BehaviorTree.Bot.Spell
{
    public class UseFreeze : Node
    {
        private AiModel bot;
        public UseFreeze(AiModel bot)
        {
            this.bot = bot;
        }

        public override NodeState Evaluate()
        {
            bot.PlaceSpell(new Model.Data.PlaceSpellData()
            {
                cardId = "",
                Xposition = bot.SpellUsingPosition.X,
                Yposition = bot.SpellUsingPosition.Y,
                stats = new Model.Data.SpellStats()
            });

            state = NodeState.RUNNING;
            return state;
        }
    }
}
