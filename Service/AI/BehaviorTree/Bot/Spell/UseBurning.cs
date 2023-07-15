using Game_Realtime.Model;
using Game_Realtime.Service.AI.BehaviorTree.Structure;
using System.Security.Cryptography.Xml;

namespace Game_Realtime.Service.AI.BehaviorTree.Bot.Spell
{
    public class UseBurning: Node
    {
        private AiModel bot;
        public UseBurning(AiModel bot)
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
