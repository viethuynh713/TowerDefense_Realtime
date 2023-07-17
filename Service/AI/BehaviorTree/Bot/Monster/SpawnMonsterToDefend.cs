using Game_Realtime.Model;
using Game_Realtime.Model.InGame;
using Game_Realtime.Service.AI.BehaviorTree.Structure;
using System.Numerics;
using System.Threading;

namespace Game_Realtime.Service.AI.BehaviorTree.Bot.Monster
{
    public class SpawnMonsterToDefend: Node
    {
        private AiModel bot;
        private MonsterModel[] monsterList;
        private Vector2 botBasePosition;

        public SpawnMonsterToDefend(AiModel bot, MonsterModel[] monsterList, Vector2 botBasePosition)
        {
            this.bot = bot;
            this.monsterList = monsterList;
            this.botBasePosition = botBasePosition;
        }

        public override NodeState Evaluate()
        {
            // get monster near castle
            foreach (var monster in monsterList)
            {
                if (monster.ownerId != bot.userId)
                {
                    if ((botBasePosition.X - monster.XLogicPosition) + (botBasePosition.Y - monster.YLogicPosition) < 3)
                    {
                        foreach (var monsterCard in bot.CardSelected)
                        {
                            if (monsterCard.Item1 == CardType.MonsterCard && monsterCard.Item2 == monster.monsterId)
                            {
                                if (bot.EnergyToSummonMonster >= AIMethod.GetEnergy(bot.CardSelected, (CardType.MonsterCard, monster.cardId)))
                                {
                                    // if bot has a monster card same as monster near castle, use that card
                                    SummonMonster(monster.cardId);
                                    state = NodeState.RUNNING;
                                    return state;
                                }
                            }
                        }
                    }
                }
            }
            // otherwise, use a random monster card
            var monsterCardList = bot.CardSelected.Where(monsterCard => monsterCard.Item1 == CardType.MonsterCard).ToList();
            if (monsterCardList.Count > 0)
            {
                var cardSelect = monsterCardList[new Random().Next(0, monsterCardList.Count)];
                while (bot.EnergyToSummonMonster < AIMethod.GetEnergy(bot.CardSelected, (CardType.MonsterCard, cardSelect.Item2)))
                {
                    monsterCardList.Remove(cardSelect);
                    cardSelect = monsterCardList[new Random().Next(0, monsterCardList.Count)];
                }
                SummonMonster(cardSelect.Item2);
            }
            state = NodeState.RUNNING;
            return state;
        }

        private void SummonMonster(string id)
        {
            // summon monster
            bot.CreateMonster(new Model.Data.CreateMonsterData()
            {
                cardId = id,
                Xposition = (int)botBasePosition.X,
                Yposition = (int)botBasePosition.Y,
                stats = new Model.Data.MonsterStats()
            });
            // cost energy
            bot.EnergyToSummonMonster -= AIMethod.GetEnergy(bot.CardSelected, (CardType.MonsterCard, id));
        }
    }
}
