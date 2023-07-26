using Game_Realtime.Model;
using Game_Realtime.Service.AI.BehaviorTree.Structure;
using Service.Models;
using System.Numerics;

namespace Game_Realtime.Service.AI.BehaviorTree.Bot.Spell
{
    public class CheckUseExplore: Node
    {
        private AiModel bot;
        private int energyRequired;
        private Vector2 botBasePosition;

        public CheckUseExplore(AiModel bot, Vector2 botBasePosition)
        {
            this.bot = bot;
            this.botBasePosition = botBasePosition;
            energyRequired = 0;
            var card = AIMethod.GetCardModel(bot.CardSelected, (CardType.SpellCard, "Explode"));
            if (card != null)
            {
                energyRequired = card.Energy;
            }
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
            // Get all enemy monsters position which is nearer 3 tiles away from base and HP > 60%
            List<Vector2> monsterNearBasePosList = new List<Vector2>();
            float HPRate = 0.6f;
            foreach (var monster in bot.GameSessionModel.GetRivalPlayer(bot.userId)._monsters)
            {
                if (MathF.Abs(botBasePosition.X - monster.Value.XLogicPosition) + MathF.Abs(botBasePosition.Y - monster.Value.YLogicPosition) < 3)
                {
                    if ((float)monster.Value.monsterHp / monster.Value.maxHp > HPRate)
                    {
                        monsterNearBasePosList.Add(new Vector2(monster.Value.XLogicPosition, monster.Value.YLogicPosition));
                    }
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
                state = NodeState.SUCCESS;
                return state;
            }

            state = NodeState.FAILURE;
            return state;
        }
    }
}
