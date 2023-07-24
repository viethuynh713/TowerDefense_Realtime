using Game_Realtime.Model;
using Game_Realtime.Service.AI.BehaviorTree.Structure;
using System.Security.Cryptography.Xml;

namespace Game_Realtime.Service.AI.BehaviorTree.Bot
{
    public class DivideEnergy: Node
    {
        private AiModel bot;

        public DivideEnergy(AiModel bot)
        {
            this.bot = bot;
        }

        public override NodeState Evaluate()
        {
            Console.WriteLine("Bot Divide Energy");

            bot.EnergyToBuildTower += bot.EnergyGain * bot.EnergyBuildTowerRate;
            bot.EnergyToSummonMonster += bot.EnergyGain - bot.EnergyToBuildTower;
            bot.EnergyGain = 0;

            state = NodeState.RUNNING;
            return state;
        }
    }
}
