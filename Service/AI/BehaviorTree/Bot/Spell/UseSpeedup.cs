using Game_Realtime.Model;
using Game_Realtime.Service.AI.BehaviorTree.Structure;
using Service.Models;

namespace Game_Realtime.Service.AI.BehaviorTree.Bot.Spell
{
    public class UseSpeedup : Node
    {
        private AiModel bot;
        private CardModel card;
        public UseSpeedup(AiModel bot)
        {
            this.bot = bot;
            var c = AIMethod.GetCardModel(bot.CardSelected, (CardType.SpellCard, "Speed"));
            if (c != null)
            {
                card = c;
            }
            else
            {
                card = new CardModel();
            }
        }

        public override NodeState Evaluate()
        {
            Console.WriteLine("Use Speed at (" + bot.SpellUsingPosition.X.ToString() + ", " + bot.SpellUsingPosition.Y.ToString() + ")");
            // use spell
            bot.GameSessionModel.PlaceSpell(bot.userId, new Model.Data.PlaceSpellData()
            {
                cardId = card.CardId,
                Xposition = bot.SpellUsingPosition.X,
                Yposition = bot.SpellUsingPosition.Y,
                stats = new Model.Data.SpellStats
                {
                    Energy = card.Energy
                }
            });
            // cost energy
            bot.EnergyToBuildTower -= card.Energy * bot.EnergyBuildTowerRate;
            bot.EnergyToSummonMonster -= card.Energy * (1 - bot.EnergyBuildTowerRate);

            state = NodeState.RUNNING;
            return state;
        }
    }
}
