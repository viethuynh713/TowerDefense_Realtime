﻿using Game_Realtime.Model;
using Game_Realtime.Model.InGame;
using Game_Realtime.Service.AI.BehaviorTree.Structure;
using System.Numerics;

namespace Game_Realtime.Service.AI.BehaviorTree.Bot.Monster
{
    public class SpawnMonsterToAttack: Node
    {
        private AiModel bot;
        private Vector2Int monsterGatePos;
        private MonsterModel[] monsterList;

        public SpawnMonsterToAttack(AiModel bot, Vector2Int monsterGatePos, MonsterModel[] monsterList)
        {
            this.bot = bot;
            this.monsterGatePos = monsterGatePos;
            this.monsterList = monsterList;
        }

        public override NodeState Evaluate()
        {
            // NOTE: check energy before using card
            // get a monster to calculate for summoning
            string checkMonsterId = "";
            foreach (var monster in monsterList)
            {
                if (MathF.Abs(monsterGatePos.x - monster.XLogicPosition) + MathF.Abs(monsterGatePos.y - monster.YLogicPosition) < 3)
                {
                    if ((checkMonsterId == "" || new Random().Next(0, 2) == 0) /* && bot.EnergyToSummonMonster >= energy[monster.monsterId] */)
                    {
                        checkMonsterId = monster.monsterId;
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
                if (bot.CardSelected.Contains((CardType.MonsterCard, checkMonsterId)) /* && bot.EnergyToSummonMonster >= energy[checkMonsterId] */)
                {
                    SummonMonster(checkMonsterId);
                    state = NodeState.RUNNING;
                    return state;
                }
            }

            if (mode == 1)
            {
                // get a 'support' card from sample list, if having card, summon it
                if (AIConstant.supportMonster.TryGetValue(checkMonsterId, out var supportMonsterIdList))
                {
                    foreach (var monsterId in supportMonsterIdList)
                    {
                        if (bot.CardSelected.Contains((CardType.MonsterCard, monsterId)))
                        {
                            SummonMonster(monsterId);
                            state = NodeState.RUNNING;
                            return state;
                        }
                    }
                }
            }
            // otherwise, use a random monster card
            var monsterCardList = bot.CardSelected.Where(monsterCard => monsterCard.Item1 == CardType.MonsterCard).ToList();
            if (monsterCardList.Count > 0)
            {
                var cardSelect = monsterCardList[new Random().Next(0, monsterCardList.Count)];
                while (false /* bot.EnergyToSummonMonster < energy[cardSelect.Item2] */)
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
            int energy = 1; // get energy to use monster card by id
            int nMonster = (int)bot.EnergyToSummonMonster / energy;
            for (int i = 0; i < nMonster; i++)
            {
                bot.CreateMonster(new Model.Data.CreateMonsterData()
                {
                    cardId = id,
                    Xposition = monsterGatePos.x,
                    Yposition = monsterGatePos.y,
                    stats = new Model.Data.MonsterStats()
                });
            }
            bot.EnergyToSummonMonster -= energy * nMonster;
        }
    }
}
