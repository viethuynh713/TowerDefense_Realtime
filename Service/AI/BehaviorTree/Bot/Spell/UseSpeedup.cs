using Game_Realtime.Model;
using Game_Realtime.Model.InGame;
using Game_Realtime.Service.AI.BehaviorTree.Structure;

namespace Game_Realtime.Service.AI.BehaviorTree.Bot.Spell
{
    public class UseSpeedup : Node
    {
        private AiModel bot;
        private int energyRequired;
        public UseSpeedup(AiModel bot, int energyRequired)
        {
            this.bot = bot;
            this.energyRequired = energyRequired;
        }

        public override NodeState Evaluate()
        {
            Console.WriteLine("Use Speed at (" + bot.SpellUsingPosition.X.ToString() + ", " + bot.SpellUsingPosition.Y.ToString() + ")");
            // use spell
            bot.GameSessionModel.PlaceSpell(bot.userId, new Model.Data.PlaceSpellData()
            {
                cardId = AIMethod.GetCardId(bot.CardSelected, (CardType.SpellCard, "Speed")),
                Xposition = bot.SpellUsingPosition.X,
                Yposition = bot.SpellUsingPosition.Y,
                stats = new Model.Data.SpellStats()
            });
            // cost energy
            bot.EnergyToBuildTower -= energyRequired * bot.EnergyBuildTowerRate;
            bot.EnergyToSummonMonster -= energyRequired * (1 - bot.EnergyBuildTowerRate);

            state = NodeState.RUNNING;
            return state;
        }
    }
}
