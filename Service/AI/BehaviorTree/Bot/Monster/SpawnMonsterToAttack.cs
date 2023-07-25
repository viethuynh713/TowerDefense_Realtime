using Game_Realtime.Model;
using Game_Realtime.Model.InGame;
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
                        if (checkMonsterCardId == "" || new Random().Next(0, 2) == 0)
                        {
                            checkMonsterCardId = monster.Value.cardId;
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
                if (AIMethod.IsBotCardSelectedContain(bot.CardSelected, (CardType.MonsterCard, checkMonsterCardId))
                    && bot.EnergyToSummonMonster >= AIMethod.GetEnergy(bot.CardSelected, (CardType.MonsterCard, checkMonsterCardId)))
                {
                    SummonMonster(checkMonsterCardId);
                    state = NodeState.RUNNING;
                    return state;
                }
            }

            if (mode == 1)
            {
                // get a 'support' card from sample list, if having card, summon it
                if (AIConstant.supportMonster.TryGetValue(checkMonsterCardId, out var supportMonsterNameList))
                {
                    foreach (var monsterName in supportMonsterNameList)
                    {
                        if (AIMethod.IsBotCardSelectedContain(bot.CardSelected, (CardType.MonsterCard, monsterName)))
                        {
                            SummonMonster(AIMethod.GetCardId(bot.CardSelected, (CardType.MonsterCard, monsterName)));
                            state = NodeState.RUNNING;
                            return state;
                        }
                    }
                }
            }
            // otherwise, use a random monster card
            var monsterCardList = bot.CardSelected.Where(monsterCard => monsterCard.Item2 == CardType.MonsterCard).ToList();
            if (monsterCardList.Count > 0)
            {
                var cardSelect = monsterCardList[new Random().Next(0, monsterCardList.Count)];
                int monsterEnergy = AIMethod.GetEnergy(bot.CardSelected, (CardType.MonsterCard, cardSelect.Item3));
                while (bot.EnergyToSummonMonster < monsterEnergy)
                {
                    monsterCardList.Remove(cardSelect);
                    cardSelect = monsterCardList[new Random().Next(0, monsterCardList.Count)];
                }
                SummonMonster(cardSelect.Item1);
            }
            state = NodeState.RUNNING;
            return state;
        }

        private async Task SummonMonster(string id)
        {
            Console.WriteLine("Spawn Monster " + id + " To Attack");
            // get cost to use a monster card
            int energy = AIMethod.GetEnergy(bot.CardSelected, (CardType.MonsterCard, id));
            // get maximum monsters can be summoned
            int nMonster = (int)bot.EnergyToSummonMonster / energy;
            for (int i = 0; i < nMonster; i++)
            {
                await bot.GameSessionModel.CreateMonster(bot.userId, new Model.Data.CreateMonsterData()
                {
                    cardId = id,
                    Xposition = monsterGatePos.x - 1,
                    Yposition = monsterGatePos.y,
                    stats = new Model.Data.MonsterStats()
                });
            }
            // cost energy
            bot.EnergyToSummonMonster -= energy * nMonster;
        }
    }
}
