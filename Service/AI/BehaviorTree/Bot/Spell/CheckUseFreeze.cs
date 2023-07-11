using Game_Realtime.Model;
using Game_Realtime.Service.AI.BehaviorTree.Structure;
using System.Numerics;
using System.Threading;

namespace Game_Realtime.Service.AI.BehaviorTree.Bot.Spell
{
    public class CheckUseFreeze: Node
    {
        private BotBTData data;
        private int energyRequired;
        private string playerId;
        private MonsterModel[] monsterList;

        public CheckUseFreeze(ref BotBTData data, int energyRequired, string playerId, MonsterModel[] monsterList)
        {
            this.data = data;
            this.energyRequired = energyRequired;
            this.playerId = playerId;
            this.monsterList = monsterList;
        }

        public override NodeState Evaluate()
        {
            //// Check if at tile which has the most tower surround has at least 3 monsters greater than 70% HP, set position to use freeze
            // check if enough energy to use
            if (data.energyToBuildTower + data.energyToSummonMonster < energyRequired)
            {
                state = NodeState.FAILURE;
                return state;
            }
            // find the tile which has the most tower surround
            Vector2Int mostTowerTilePos = new Vector2Int(0, 0);
            int nTowerAtMostTowerTile = 0;
            for (int i = 0; i < data.towerBuildingMapHeight; i++)
            {
                for (int j = 0; j < data.towerBuildingMapWidth; j++)
                {
                    int nTower = 0;
                    for (int x = -1; x <= 1; x++)
                    {
                        for (int y = -1; y <= 1; y++)
                        {
                            if (data.towerBuildingMap[i + y][j + x].hasTower)
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
            foreach (var monster in monsterList)
            {
                if (monster.ownerId == playerId)
                {
                    if (MathF.Abs(monster.XLogicPosition - mostTowerTilePos.x) <= 1.5
                        && MathF.Abs(monster.YLogicPosition - mostTowerTilePos.y) <= 1.5
                        && monster.monsterHp / monster.maxHp > HPRate)
                    {
                        nMonster++;
                    }
                }
            }
            if (nMonster >= 3)
            {
                data.spellUsingPosition = new Vector2(mostTowerTilePos.x, mostTowerTilePos.y);
                data.spellUsingName = "Freeze";
                state = NodeState.SUCCESS;
                return state;
            }

            state = NodeState.FAILURE;
            return state;
        }
    }
}
