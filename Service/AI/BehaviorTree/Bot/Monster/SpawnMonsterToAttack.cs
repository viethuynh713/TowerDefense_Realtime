using Game_Realtime.Model;
using Service.Models;
using Game_Realtime.Service.AI.BehaviorTree.Structure;
using System.Numerics;

namespace Game_Realtime.Service.AI.BehaviorTree.Bot.Monster
{
    public class SpawnMonsterToAttack: Node
    {
        private AiModel bot;
        private Vector2Int monsterGatePos;

        public SpawnMonsterToAttack(AiModel bot, Vector2Int monsterGatePos)
        {
            this.bot = bot;
            this.monsterGatePos = monsterGatePos;
        }

        public override NodeState Evaluate()
        {
            Console.WriteLine("SpawnMonsterToAttack");
            // get a monster to calculate for summoning
            string checkMonsterCardId = "";
            foreach (var monster in bot._monsters)
            {
                if (monster.Value.ownerId == bot.userId)
                {
                    if (MathF.Abs(monsterGatePos.x - monster.Value.XLogicPosition) + MathF.Abs(monsterGatePos.y - monster.Value.YLogicPosition) < 3)
                    {
                        if (checkMonsterCardId == ""/* || new Random().Next(0, 2) == 0*/)
                        {
                            checkMonsterCardId = monster.Value.cardId;
                            break;
                        }
                    }
                }
            }
            // choose mode
            // 0 is summon a same monster as the monster group already summoned
            // 1 is summon a 'support' monster with the monster group already summoned
            // and spawn some monsters at monster wave gates
            int mode = new Random().Next(0, 2);
            if (mode == 0)
            {
                // if having card as same as monster to summon, summon it
                var card = AIMethod.GetCardModel(bot.CardSelected, (CardType.MonsterCard, checkMonsterCardId));
                if (card != null) {
                    if (bot.EnergyToSummonMonster >= card.Energy)
                    {
                        SummonMonster(card);
                        state = NodeState.RUNNING;
                        return state;
                    }
                }
            }

            if (mode == 1)
            {
                // get a 'support' card from sample list, if having card, summon it
                if (AIConstant.supportMonster.TryGetValue(checkMonsterCardId, out var supportMonsterNameList))
                {
                    foreach (var monsterName in supportMonsterNameList)
                    {
                        var card = AIMethod.GetCardModel(bot.CardSelected, (CardType.MonsterCard, monsterName));
                        if (card != null)
                        {
                            SummonMonster(card);
                            state = NodeState.RUNNING;
                            return state;
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

        private async Task SummonMonster(CardModel card)
        {
            Console.WriteLine("Spawn Monster " + card.CardId + " To Attack");
            // get maximum monsters can be summoned
            int nMonster = (int)bot.EnergyToSummonMonster / card.Energy;
            for (int i = 0; i < nMonster; i++)
            {
                await bot.GameSessionModel.CreateMonster(bot.userId, new Model.Data.CreateMonsterData()
                {
                    cardId = card.CardId,
                    Xposition = monsterGatePos.x + 1,
                    Yposition = monsterGatePos.y,
                    stats = new Model.Data.MonsterStats
                    {
                        Energy = card.Energy
                    }
                });
            }
            // cost energy
            bot.EnergyToSummonMonster -= card.Energy * nMonster;
        }
    }
}
