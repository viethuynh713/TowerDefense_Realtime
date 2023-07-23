using Game_Realtime.Model;
using Game_Realtime.Service.AI.BehaviorTree.Structure;
using System.Numerics;

namespace Game_Realtime.Service.AI.BehaviorTree.Bot.Monster
{
    public class CheckSpawnMonsterToDefend: Node
    {
        private AiModel bot;
        private int minEnergyRequired;
        private Vector2 botBasePosition;

        public CheckSpawnMonsterToDefend(AiModel bot, Vector2 botBasePosition)
        {
            this.bot = bot;
            minEnergyRequired = AIMethod.GetMinMonsterEnergy(bot.CardSelected);
            this.botBasePosition = botBasePosition;
        }

        public override NodeState Evaluate()
        {
            // check if enough energy to use
            if (bot.EnergyToSummonMonster <= minEnergyRequired)
            {
                state = NodeState.FAILURE;
                return state;
            }
            // if castle is low HP or playmode is not defend
            if (bot.PlayMode != BotPlayMode.DEFEND || bot.castleHp < GameConfig.GameConfig.MAX_CASTLE_HP * 0.3f)
            {
                // and any opponent monsters are near castle, summon a monster at castle to defend
                foreach (var monster in bot.GameSessionModel.GetRivalPlayer(bot.userId)._monsters)
                {
                    if ((botBasePosition.X - monster.Value.XLogicPosition) + (botBasePosition.Y - monster.Value.YLogicPosition) < 3)
                    {
                        state = NodeState.SUCCESS;
                        return state;
                    }
                }
            }

            state = NodeState.FAILURE;
            return state;
        }
    }
}
