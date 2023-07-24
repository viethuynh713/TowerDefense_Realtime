using Game_Realtime.Model;
using Game_Realtime.Service.AI.BehaviorTree.Structure;
using System.Numerics;

namespace Game_Realtime.Service.AI.BehaviorTree.Bot.Spell
{
    public class CheckUseExplore: Node
    {
        private AiModel bot;
        private int energyRequired;
        private Vector2 botBasePosition;

        public CheckUseExplore(AiModel bot, int energyRequired, Vector2 botBasePosition)
        {
            this.bot = bot;
            this.energyRequired = energyRequired;
            this.botBasePosition = botBasePosition;
        }

        public override NodeState Evaluate()
        {
            //// Check if any monster is nearer 3 tiles away from base, set position to use explore
            // check if enough energy to use
            if (bot.EnergyToBuildTower + bot.EnergyToSummonMonster < energyRequired)
            {
                state = NodeState.FAILURE;
                return state;
            }
            // Get all enemy monsters position which is nearer 3 tiles away from base
            List<Vector2> monsterNearBasePosList = new List<Vector2>();
            foreach (var monster in bot.GameSessionModel.GetRivalPlayer(bot.userId)._monsters)
            {
                if (MathF.Abs(botBasePosition.X - monster.Value.XLogicPosition) + MathF.Abs(botBasePosition.Y - monster.Value.YLogicPosition) < 3)
                {
                    monsterNearBasePosList.Add(new Vector2(monster.Value.XLogicPosition, monster.Value.YLogicPosition));
                }
            }
            // Use spell at the center of the monsters found
            if (monsterNearBasePosList.Count > 0)
            {
                bot.SpellUsingPosition = new Vector2(0, 0);
                foreach (var pos in monsterNearBasePosList)
                {
                    bot.SpellUsingPosition += pos / monsterNearBasePosList.Count;
                }
                bot.SpellUsingName = "Explode";
                state = NodeState.SUCCESS;
                return state;
            }

            state = NodeState.FAILURE;
            return state;
        }
    }
}
