using Game_Realtime.Model.InGame;
using Game_Realtime.Service.AI.BehaviorTree.Structure;
using Game_Realtime.Service.AI.TowerBuildingMapService;
using Game_Realtime.Service.AI.BehaviorTree.Bot.Spell;
using Game_Realtime.Model;

namespace Game_Realtime.Service.AI.BehaviorTree.Bot
{
    public class BotBT: Tree
    {
        BotBTData data = new BotBTData();
        protected override Node SetUpTree()
        {
            List<Node> spellNodeList = new List<Node>();
            switch (data.playMode)
            {
                case BotPlayMode.ATTACK:
                    AddSpellBehavior(ref spellNodeList, "Speedup");
                    AddSpellBehavior(ref spellNodeList, "Healing");
                    AddSpellBehavior(ref spellNodeList, "Burning");
                    AddSpellBehavior(ref spellNodeList, "Explore");
                    break;
                case BotPlayMode.DEFEND:
                    AddSpellBehavior(ref spellNodeList, "Explore");
                    AddSpellBehavior(ref spellNodeList, "Freeze");
                    AddSpellBehavior(ref spellNodeList, "Burning");
                    AddSpellBehavior(ref spellNodeList, "Healing");
                    AddSpellBehavior(ref spellNodeList, "Speedup");
                    break;
                case BotPlayMode.HYBRIC:
                    AddSpellBehavior(ref spellNodeList, "Burning");
                    AddSpellBehavior(ref spellNodeList, "Healing");
                    AddSpellBehavior(ref spellNodeList, "Explore");
                    AddSpellBehavior(ref spellNodeList, "Speedup");
                    AddSpellBehavior(ref spellNodeList, "Freeze");
                    break;
            }

            Node root = new Sequence(new List<Node>
            {
                new DivideEnergy(ref data),
                new Selector(spellNodeList)
            });

            return root;
        }

        public void SetData(BotPlayMode playMode, List<(CardType, string)> cardSelected, BotLogicTile[][] towerBuildingMap, float energyBuildTowerRate)
        {
            data.playMode = playMode;
            data.cardSelected = cardSelected;
            data.towerBuildingMap = towerBuildingMap;
            data.towerBuildingMapWidth = towerBuildingMap[0].Length;
            data.towerBuildingMapHeight = towerBuildingMap.Length;
            data.energyBuildTowerRate = energyBuildTowerRate;
        }

        private void AddSpellBehavior(ref List<Node> spellNodeList, string spellName)
        {
            if (spellName == "Burning")
            {
                if (data.cardSelected.Contains((CardType.SpellCard, "Burning")))
                {
                    spellNodeList.Add(new Sequence(new List<Node>()
                    {
                        new CheckUseBurning(ref data, 0, "", new MonsterModel[0]),
                        new UseBurning(ref data)
                    }));
                }
            }
            if (spellName == "Explore")
            {
                if (data.cardSelected.Contains((CardType.SpellCard, "Explore")))
                {
                    spellNodeList.Add(new Sequence(new List<Node>()
                    {
                        new CheckUseExplore(ref data, 0, "", new MonsterModel[0], new System.Numerics.Vector2(0, 0)),
                        new UseExplore(ref data)
                    }));
                }
            }
            if (spellName == "Freeze")
            {
                if (data.cardSelected.Contains((CardType.SpellCard, "Freeze")))
                {
                    spellNodeList.Add(new Sequence(new List<Node>() {
                        new CheckUseFreeze(ref data, 0, "", new MonsterModel[0]),
                        new UseFreeze(ref data)
                    })); ;
                }
            }
            if (spellName == "Healing")
            {
                if (data.cardSelected.Contains((CardType.SpellCard, "Healing")))
                {
                    spellNodeList.Add(new Sequence(new List<Node>() {
                        new CheckUseHealing(ref data, 0, "", new MonsterModel[0]),
                        new UseHealing(ref data)
                    })); 
                }
            }
            if (spellName == "Speedup")
            {
                if (data.cardSelected.Contains((CardType.SpellCard, "Speedup")))
                {
                    spellNodeList.Add(new Sequence(new List<Node>()
                    {
                        new CheckUseSpeedup(ref data, 0, "", new MonsterModel[0], new System.Numerics.Vector2(0, 0)),
                        new UseSpeedup(ref data)
                    }));
                }
            }
        }
    }
}
