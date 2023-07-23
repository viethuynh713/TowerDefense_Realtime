using Game_Realtime.Model;
using Game_Realtime.Service.AI.BehaviorTree.Structure;
using System.Numerics;
using System.Threading;

namespace Game_Realtime.Service.AI.BehaviorTree.Bot.Spell
{
    public class CheckUseFreeze: Node
    {
        private AiModel bot;
        private int energyRequired;

        public CheckUseFreeze(AiModel bot, int energyRequired)
        {
            this.bot = bot;
            this.energyRequired = energyRequired;
        }

        public override NodeState Evaluate()
        {
            //// Check if at tile which has the most tower surround has at least 3 monsters greater than 70% HP, set position to use freeze
            // check if enough energy to use
            if (bot.EnergyToBuildTower + bot.EnergyToSummonMonster < energyRequired)
            {
                state = NodeState.FAILURE;
                return state;
            }
            // find the tile which has the most tower surround
            Vector2Int mostTowerTilePos = new Vector2Int(0, 0);
            int nTowerAtMostTowerTile = 0;
            for (int i = 0; i < bot.TowerBuildingMapHeight; i++)
            {
                for (int j = 0; j < bot.TowerBuildingMapWidth; j++)
                {
                    int nTower = 0;
                    for (int x = -1; x <= 1; x++)
                    {
                        for (int y = -1; y <= 1; y++)
                        {
                            if (bot.TowerBuildingMap[i + y][j + x].hasTower != null
                                && bot.TowerBuildingMap[i + y][j + x].hasTower.Value)
                            {
                                nTower++;
                            }
                        }
                    }
                    int rand = new Random().Next(0, 2);
                    if (nTower > nTowerAtMostTowerTile || (nTower == nTowerAtMostTowerTile && rand == 0))
                    {
                        mostTowerTilePos = new Vector2Int(j, i);
                        nTowerAtMostTowerTile = nTower;
                    }
                }
            }
            // check if at least 3 monster with HP greater than 70% nearby this tile
            int nMonster = 0;
            float HPRate = 0.7f;
            foreach (var monster in bot.GameSessionModel.GetRivalPlayer(bot.userId)._monsters)
            {
                if (monster.Value.ownerId != bot.userId)
                {
                    if (MathF.Abs(monster.Value.XLogicPosition - mostTowerTilePos.x) <= 1.5
                        && MathF.Abs(monster.Value.YLogicPosition - mostTowerTilePos.y) <= 1.5
                        && monster.Value.monsterHp / monster.Value.maxHp > HPRate)
                    {
                        nMonster++;
                    }
                }
            }
            if (nMonster >= 3)
            {
                bot.SpellUsingPosition = new Vector2(mostTowerTilePos.x, mostTowerTilePos.y);
                bot.SpellUsingName = "Freeze";
                state = NodeState.SUCCESS;
                return state;
            }

            state = NodeState.FAILURE;
            return state;
        }
    }
}
