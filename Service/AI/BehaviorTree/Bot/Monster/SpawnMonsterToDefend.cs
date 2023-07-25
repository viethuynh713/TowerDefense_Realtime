using Game_Realtime.Model;
using Service.Models;
using Game_Realtime.Service.AI.BehaviorTree.Structure;
using System.Numerics;
using System.Threading;
using Newtonsoft.Json;

namespace Game_Realtime.Service.AI.BehaviorTree.Bot.Monster
{
    public class SpawnMonsterToDefend: Node
    {
        private AiModel bot;
        private Vector2 botBasePosition;
        private List<CardModel> stockCards;

        public SpawnMonsterToDefend(AiModel bot, Vector2 botBasePosition)
        {
            this.bot = bot;
            this.botBasePosition = botBasePosition;

            var json = File.ReadAllText("./CardConfig/MythicEmpire.Cards.json");
            var stockCards = JsonConvert.DeserializeObject<List<CardModel>>(json);
            if (stockCards == null) Console.WriteLine("Get MythicEmpire.Cards Error!");
        }

        public override NodeState Evaluate()
        {
            // get monster near castle
            foreach (var monster in bot.GameSessionModel.GetRivalPlayer(bot.userId)._monsters)
            {
                if ((botBasePosition.X - monster.Value.XLogicPosition) + (botBasePosition.Y - monster.Value.YLogicPosition) < 3)
                {
                    foreach (var monsterCard in bot.CardSelected)
                    {
                        CardModel sampleCard = stockCards.FirstOrDefault(c => c.CardId == monster.Value.monsterId);
                        if (sampleCard != null)
                        {
                            string cardName = sampleCard.CardName;
                            if (monsterCard.TypeOfCard == CardType.MonsterCard && monsterCard.CardName == cardName)
                            {
                                CardModel card = AIMethod.GetCardModel(bot.CardSelected, (CardType.MonsterCard, cardName));
                                if (bot.EnergyToSummonMonster >= card.Energy)
                                {
                                    // if bot has a monster card same as monster near castle, use that card
                                    SummonMonster(card);
                                    state = NodeState.RUNNING;
                                    return state;
                                }
                            }
                        }
                    }
                }
            }
            // otherwise, use a random monster card
            var monsterCardList = bot.CardSelected.Where(monsterCard => monsterCard.TypeOfCard == CardType.MonsterCard).ToList();
            if (monsterCardList.Count > 0)
            {
                var cardSelect = monsterCardList[new Random().Next(0, monsterCardList.Count)];
                while (bot.EnergyToSummonMonster < cardSelect.Energy)
                {
                    monsterCardList.Remove(cardSelect);
                    cardSelect = monsterCardList[new Random().Next(0, monsterCardList.Count)];
                }
                SummonMonster(cardSelect);
            }
            state = NodeState.RUNNING;
            return state;
        }

        private void SummonMonster(CardModel card)
        {
            Console.WriteLine("Spawn Monster " + card.CardId + " To Defend");
            // summon monster
            bot.GameSessionModel.CreateMonster(bot.userId, new Model.Data.CreateMonsterData()
            {
                cardId = card.CardId,
                Xposition = (int)botBasePosition.X - 1,
                Yposition = (int)botBasePosition.Y,
                stats = new Model.Data.MonsterStats
                {
                    Energy = card.Energy
                }
            });
            // cost energy
            bot.EnergyToSummonMonster -= card.Energy;
        }
    }
}
