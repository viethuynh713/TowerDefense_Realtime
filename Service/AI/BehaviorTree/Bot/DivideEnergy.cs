using Game_Realtime.Service.AI.BehaviorTree.Structure;
using System.Security.Cryptography.Xml;

namespace Game_Realtime.Service.AI.BehaviorTree.Bot
{
    public class DivideEnergy: Node
    {
        private BotBTData data;
        private float energyBuildTowerRate;

        public DivideEnergy(ref BotBTData data)
        {
            this.data = data;
            energyBuildTowerRate = data.energyBuildTowerRate;
        }

        public override NodeState Evaluate()
        {
            data.energyToBuildTower += data.energyGain * energyBuildTowerRate;
            data.energyToSummonMonster += data.energyGain - data.energyToBuildTower;
            data.energyGain = 0;

            state = NodeState.RUNNING;
            return state;
        }
    }
}
