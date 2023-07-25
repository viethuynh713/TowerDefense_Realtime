using Game_Realtime.Model.Data;
using Game_Realtime.Model.Map;
using Game_Realtime.Service;
using Game_Realtime.Service.AI;
using Game_Realtime.Service.AI.BehaviorTree.Bot;
using Game_Realtime.Service.AI.SelectCardService;
using Game_Realtime.Service.AI.TowerBuildingMapService;
using Newtonsoft.Json;
using Service.Models;
using System.Linq;
using System.Numerics;

namespace Game_Realtime.Model;

public class AiModel: BasePlayer
{
    private float energyToBuildTower;
    private float energyToSummonMonster;
    private int energyGain;
    private Vector2 spellUsingPosition;
    private bool hasAutoSummonMonsterCurrently;
    private Vector2Int? towerSelectPos;

    private BotPlayMode playMode;
    private List<CardModel> cardSelected;
    private BotLogicTile[][] towerBuildingMap;
    private int towerBuildingMapWidth;
    private int towerBuildingMapHeight;
    private int realMapWidth;
    private int realMapHeight;
    private float energyBuildTowerRate;
    private string basicTowerName;
    private BotBT behavior;
    private FindTowerTypeStrategy findTowerTypeStrategy;
    private FindTowerPosStrategy findTowerPosStrategy;
    private List<Vector2Int> towerBuildOrder;
    private List<Vector2Int> towerBuildProgressOrder;
    private List<Vector2Int> longestPath;

    private GameSessionModel gameSessionModel;

    public AiModel() : base()
    {

    }
    public async void InitBot(GameSessionModel gameSessionModel)
    {
        energyToBuildTower = 0;
        energyToSummonMonster = 0;
        energyGain = energy;

        spellUsingPosition = new Vector2();

        playMode = (BotPlayMode)new Random().Next(0, 3);
        towerSelectPos = null;

        realMapWidth = gameSessionModel._mapService.Width;
        realMapHeight = gameSessionModel._mapService.Height;

        this.gameSessionModel = gameSessionModel;

        var rivalPlayer = gameSessionModel.GetRivalPlayer(userId);
        if (rivalPlayer != null && rivalPlayer is PlayerModel)
        {
            await ChooseListCard(((PlayerModel) rivalPlayer).cards);
            await CalculateEnergyRateUsing();
            await CreateTowerBuildingMap(gameSessionModel._mapService);
            await SelectBattleMode();
            behavior = new BotBT(this);
        }
    }

    public async Task<List<string>> ChooseListCard(List<string> rivalCards)
    {
        // get card json file
        var json = File.ReadAllText("./CardConfig/MythicEmpire.Cards.json");
        var stockCards = JsonConvert.DeserializeObject<List<CardModel>>(json);
        if (stockCards == null) Console.WriteLine("Get MythicEmpire.Cards Error!");
        if (stockCards != null)
        {
            // select card by mode
            cardSelected = new List<CardModel>();
            Console.WriteLine("Playmode: " + playMode.ToString());
            AIConstant.CardSelectingStrategy.TryGetValue(playMode, out var strategy);
            if (strategy != null)
            {
                foreach (var cardTypeStrategy in strategy)
                {
                    AIConstant.CardGroup.TryGetValue(cardTypeStrategy.Key, out var cardListOfType);
                    if (cardListOfType != null)
                    {
                        foreach (var nCardOfType in cardTypeStrategy.Value)
                        {
                            cardListOfType.TryGetValue(nCardOfType.Key, out var cardList);
                            if (cardList != null)
                            {
                                for (int i = 0; i < nCardOfType.Value; i++)
                                {
                                    string cardName = cardList[new Random().Next(0, cardList.Length)];
                                    Console.WriteLine(cardName);
                                    var card = stockCards.FirstOrDefault(c => c.CardName == cardName && c.TypeOfCard == cardTypeStrategy.Key);
                                    if (card != null)
                                    {
                                        cardSelected.Add(new CardModel
                                        {
                                            TypeOfCard = card.TypeOfCard,
                                            CardName = cardName,
                                        });
                                    }
                                }
                            }
                        }
                    }
                }
            }
            // get player cards' rarity
            Dictionary<RarityCard, int> nRarity = new Dictionary<RarityCard, int>()
            {
                { RarityCard.Legend, 0 }, { RarityCard.Mythic, 0 }, { RarityCard.Rare, 0 }, { RarityCard.Common, 0 }
            };
            int star = 0;
            float f_star = 0;
            foreach (var rivalCard in rivalCards)
            {
                var stockCard = stockCards.FirstOrDefault(c => c.CardId == rivalCard);
                if (stockCard != null)
                {
                    RarityCard rarity = stockCard.CardRarity;
                    if (nRarity.TryGetValue(rarity, out int n))
                    {
                        nRarity[rarity] = n + 1;
                    }
                    f_star += stockCard.CardStar / rivalCards.Count;
                }
            }
            star = (int)Math.Round(f_star);
            if (rivalCards.Count < cardSelected.Count)
            {
                int length = rivalCards.Count;
                Dictionary<RarityCard, int> nRarityTemp = new Dictionary<RarityCard, int>(nRarity);
                int idx = (int)RarityCard.Legend;
                while (length < cardSelected.Count)
                {
                    if (idx < 1)
                    {
                        idx = (int)RarityCard.Legend;
                    }
                    else if (nRarity.TryGetValue((RarityCard)idx, out int n))
                    {
                        int nTemp = nRarityTemp[(RarityCard)idx];
                        if (cardSelected.Count - length <= nTemp)
                        {
                            nRarity[(RarityCard)idx] = n + (cardSelected.Count - length);
                            length = cardSelected.Count;
                        }
                        else
                        {
                            nRarity[(RarityCard)idx] = n + nTemp;
                            length += nTemp;
                        }
                        idx--;
                    }
                    Console.WriteLine("While: length: " + length.ToString() + " - idx: " + idx.ToString());
                }
            }
            //// set rarity for cards
            {
                int i, j;
                CardModel temp;
                bool swapped;
                // sort card selecting list by priority (defend on bot play mode)
                for (i = 0; i < cardSelected.Count - 1; i++)
                {
                    swapped = false;
                    for (j = 0; j < cardSelected.Count - i - 1; j++)
                    {
                        if (!SelectCardService.IsGreaterThan(cardSelected[j], cardSelected[j + 1], playMode))
                        {
                            temp = cardSelected[j];
                            cardSelected[j] = cardSelected[j + 1];
                            cardSelected[j + 1] = temp;
                            swapped = true;
                        }
                    }
                    if (swapped == false)
                        break;
                }
            }
            // set rarity for cards, the more prioritized cards, the higher rarity
            List<string> cardSelectingIdList = new List<string>();
            int cardIterationIdx = 0;
            foreach (var rarity in nRarity)
            {
                Console.WriteLine(rarity.Key.ToString() + ": " + rarity.Value.ToString());
                for (int i = cardIterationIdx; i < cardIterationIdx + rarity.Value; i++)
                {
                    var stockCard = stockCards.FirstOrDefault(c =>
                        c.TypeOfCard == cardSelected[i].TypeOfCard && c.CardName == cardSelected[i].CardName
                        && c.CardStar == star && c.CardRarity == rarity.Key);
                    if (stockCard != null)
                    {
                        cardSelectingIdList.Add(stockCard.CardId);
                        cardSelected[i].CardId = stockCard.CardId;
                        cardSelected[i].Energy = stockCard.Energy;
                        cardSelected[i].CardStar = stockCard.CardStar;
                        cardSelected[i].CardRarity = stockCard.CardRarity;
                    }
                }
                cardIterationIdx += rarity.Value;
            }
            Console.WriteLine("Choose List Card: " + cardSelected.Count.ToString());
            foreach (var card in cardSelected)
            {
                Console.WriteLine(card.CardId.ToString() + ", " + card.TypeOfCard.ToString() + ", " + card.CardName.ToString() + ", " + card.Energy.ToString());
            }
            return cardSelectingIdList;
        }
        return new List<string>();
    }

    public Task CalculateEnergyRateUsing()
    {
        energyBuildTowerRate = AIConstant.EnergyBuildTowerRate[playMode];
        Console.WriteLine("EnergyBuildTowerRate: " + energyBuildTowerRate.ToString());
        return Task.CompletedTask;
            
    }
        
    public async Task CreateTowerBuildingMap(MapService map)
    {
        towerBuildingMapWidth = (map.Width - 3) / 2;
        towerBuildingMapHeight = map.Height - 2;
        // get longest path
        Console.WriteLine("Map: ");
        foreach (var row in map._logicMap)
        {
            foreach (var tile in row)
            {
                if (tile.TypeOfType == TypeTile.Barrier)
                {
                    Console.Write("#");
                }
                else
                {
                    Console.Write(".");
                }
            }
            Console.WriteLine("");
        }
        longestPath = map.InitLongestPath();
        for (int i = 0; i < longestPath.Count; i++)
        {
            longestPath[i] = new Vector2Int(longestPath[i].x - towerBuildingMapWidth - 2, longestPath[i].y - 1);
        }
        Console.WriteLine("LongestPath: " + longestPath.Count.ToString());
        foreach (var tile in longestPath)
        {
            Console.Write("(" + tile.x.ToString() + ", " + tile.y.ToString() + ") -> ");
        }
        Console.WriteLine("");
        // initial tower building map
        towerBuildingMap = new BotLogicTile[towerBuildingMapHeight][];
        for (int i = 0; i < towerBuildingMapHeight; i++)
        {
            towerBuildingMap[i] = new BotLogicTile[towerBuildingMapWidth];
        }
        // copy data from base map to tower building map
        for (int i = 0; i < towerBuildingMapHeight; i++)
        {
            for (int j = 0; j < towerBuildingMapWidth; j++)
            {
                towerBuildingMap[i][j] = new BotLogicTile();
                towerBuildingMap[i][j].Copy(map.LogicMap[i + 1][j + towerBuildingMapWidth + 2]);
            }
        }
        // build tower building map
        for (int i = 0; i < towerBuildingMapHeight; i++)
        {
            for (int j = 0; j < towerBuildingMapWidth; j++)
            {
                if (towerBuildingMap[i][j].TypeOfType == TypeTile.Normal)
                {
                    if (longestPath.Contains(new Vector2Int(j, i)))
                    {
                        towerBuildingMap[i][j].isBuildTower = false;
                    }
                    else
                    {
                        towerBuildingMap[i][j].isBuildTower = true;
                        towerBuildingMap[i][j].hasTower = false;
                        // count number of adjacent path tile
                        var adjacentTile = new Vector2Int[] { new Vector2Int(j - 1, i), new Vector2Int(j + 1, i), new Vector2Int(j, i - 1),
                            new Vector2Int(j, i + 1), new Vector2Int(j - 1, i - 1), new Vector2Int(j - 1, i + 1),
                            new Vector2Int(j + 1, i - 1), new Vector2Int(j + 1, i + 1)};
                        int nAdjacentTile = 0;
                        foreach (var tile in adjacentTile)
                        {
                            if (longestPath.Contains(tile))
                            {
                                nAdjacentTile++;
                            }
                        }
                        // get a random tower from tower group base on number of adjacent path tile
                        int towerGroupTier = 0;
                        if (nAdjacentTile < 2) towerGroupTier = 0;
                        else if (nAdjacentTile < 4) towerGroupTier = 1;
                        else if (nAdjacentTile < 6) towerGroupTier = 2;
                        else towerGroupTier = 3;
                        List<string> towerCanChose = FindCardSelected(AIConstant.TowerTier[towerGroupTier], CardType.TowerCard);
                        towerBuildingMap[i][j].towerName = towerCanChose[new Random().Next(0, towerCanChose.Count)];
                    }
                } 
            }
        }
        Console.WriteLine("TowerBuildingMap: ");
        foreach (var row in towerBuildingMap)
        {
            foreach (var tile in row)
            {
                if (tile.TypeOfType == TypeTile.Barrier)
                {
                    Console.Write("#\t");
                }
                else
                {
                    if (tile.isBuildTower)
                    {
                        Console.Write(tile.towerName[0] + tile.towerName.Length.ToString() + "\t");
                    }
                    else
                    {
                        Console.Write(".\t");
                    }
                }
            }
            Console.WriteLine("");
        }
    }

    private List<string> FindCardSelected(List<string> idList, CardType cardType)
    {
        // Get all the card both in card selected and card tier group.
        // If having any, return them, otherwise, return all card selected.
        // This function supports to find tower for building tower building map.
        var result = new List<string>();
        var otherResult = new List<string>();
        foreach (var card in cardSelected)
        {
            if (card.TypeOfCard == cardType)
            {
                otherResult.Add(card.CardName);
                if (idList.Contains(card.CardName))
                {
                    result.Add(card.CardName);
                }
            }
        }
        if (result.Count > 0)
        {
            return result;
        }
        return otherResult;
    }

    private Task SelectBattleMode()
    {
        // select find tower type strategy
        findTowerTypeStrategy = (FindTowerTypeStrategy)new Random().Next(0, 2);
        // select find tower position strategy
        findTowerPosStrategy = (FindTowerPosStrategy)new Random().Next(0, 4);
        Console.WriteLine("FindTowerTypeStrategy: " + findTowerTypeStrategy.ToString());
        Console.WriteLine("FindTowerPosStrategy: " + findTowerPosStrategy.ToString());
        // create tower building order
        // the first is position
        // the second is strength (get by index in AIConstant.towerStrength)
        // the third is number of empty adjacent node which is not used to build tower
        towerBuildOrder = new List<Vector2Int>();
        if (findTowerPosStrategy == FindTowerPosStrategy.WEAK_TO_STRONG || findTowerPosStrategy == FindTowerPosStrategy.STRONG_TO_WEAK)
        {
            List<(Vector2Int, int, int)> calcTowerBuildOrder = new List<(Vector2Int, int, int)>();
            for (int i = 0; i < towerBuildingMapHeight; i++)
            {
                for (int j = 0; j < towerBuildingMapWidth; j++)
                {
                    var tile = towerBuildingMap[i][j];
                    if (tile.isBuildTower)
                    {
                        // get number of empty adjacent tile which is not used to build tower
                        int nEmptyAdjacentTile = 0;
                        for (int x = -1; x <= 1; x++)
                        {
                            if (j + x < 0 || j + x >= towerBuildingMapWidth)
                            {
                                continue;
                            }
                            for (int y = -1; y <= 1; y++)
                            {
                                if (i + y >= 0 && i + y < towerBuildingMapHeight)
                                {
                                    if (!tile.isBuildTower)
                                    {
                                        nEmptyAdjacentTile++;
                                    }
                                }
                            }
                        }
                        // get data to compare
                        (Vector2Int, int, int) newNode = (
                            new Vector2Int(j, i),
                            AIConstant.towerStrength.FindIndex(node => node.Contains(towerBuildingMap[i][j].towerName)),
                            nEmptyAdjacentTile
                        );
                        // put new node to suitable position (sort)
                        bool hasInserted = false;
                        for (int k = 0; k < calcTowerBuildOrder.Count; k++)
                        {
                            bool condition = findTowerPosStrategy == FindTowerPosStrategy.WEAK_TO_STRONG ?
                                newNode.Item2 < calcTowerBuildOrder[k].Item2 :
                                newNode.Item2 > calcTowerBuildOrder[k].Item2;
                            if (condition)
                            {
                                calcTowerBuildOrder.Insert(k, newNode);
                                hasInserted = true;
                                break;
                            }
                            if (newNode.Item2 == calcTowerBuildOrder[k].Item2)
                            {
                                if (newNode.Item3 > calcTowerBuildOrder[k].Item3)
                                {
                                    calcTowerBuildOrder.Insert(k, newNode);
                                    hasInserted = true;
                                    break;
                                }
                            }
                        }
                        if (!hasInserted)
                        {
                            calcTowerBuildOrder.Add(newNode);
                        }
                    }
                }
            }
            // copy to tower building order list
            foreach (var node in calcTowerBuildOrder)
            {
                towerBuildOrder.Add(node.Item1);
            }
        }
        else
        {
            // get list of tile which will be used to build tower
            List<Vector2Int> towerBuildingTileList = new List<Vector2Int>();
            for (int i = 0; i < towerBuildingMapHeight; i++)
            {
                for (int j = 0; j < towerBuildingMapWidth; j++)
                {
                    var _tile = towerBuildingMap[i][j];
                    if (_tile.isBuildTower)
                    {
                        towerBuildingTileList.Add(new Vector2Int(j, i));
                    }
                }
            }
            // add tile across longest path to order from gate to castle
            foreach (var pathTile in longestPath)
            {
                for (int x = -1; x <= 1; x++)
                {
                    if (pathTile.x + x >= 0 && pathTile.x + x < towerBuildingMapWidth)
                    {
                        for (int y = -1; y <= 1; y++)
                        {
                            if (pathTile.y + y >= 0 && pathTile.y + y < towerBuildingMapWidth)
                            {
                                if (towerBuildingMap[pathTile.y + y][pathTile.x + x].isBuildTower
                                && !towerBuildOrder.Contains(new Vector2Int(pathTile.x + x, pathTile.y + y)))
                                {
                                    towerBuildOrder.Add(new Vector2Int(pathTile.x + x, pathTile.y + y));
                                    towerBuildingTileList.Remove(new Vector2Int(pathTile.x + x, pathTile.y + y));
                                }
                            }
                        }
                    }
                }
            }
            // if bot build tower from castle to gate, reverse the order
            if (findTowerPosStrategy == FindTowerPosStrategy.CASTLE_TO_GATE)
            {
                towerBuildOrder.Reverse();
            }
            // add tiles not adject longest path to list
            foreach (var restTile in towerBuildingTileList)
            {
                towerBuildOrder.Add(restTile);
            }
        }
        // create a clone list tower to build again if tower building type is PROGRESS, this list don't have tile with build basic tower
        towerBuildProgressOrder = new List<Vector2Int>();
        basicTowerName = "";
        if (findTowerTypeStrategy == FindTowerTypeStrategy.PROGRESS)
        {
            foreach (var towerList in AIConstant.AttackTowerTier)
            {
                bool found = false;
                foreach (var towerName in towerList)
                {
                    if (AIMethod.IsBotCardSelectedContain(cardSelected, (CardType.TowerCard, towerName)))
                    {
                        basicTowerName = towerName;
                        found = true;
                        break;
                    }
                }
                if (found) break;
            }
            if (basicTowerName == "")
            {
                basicTowerName = "Energy";
            }
        }
        foreach (var tilePos in towerBuildOrder)
        {
            if (towerBuildingMap[tilePos.y][tilePos.x].towerName != basicTowerName)
            {
                towerBuildProgressOrder.Add(tilePos);
            }
        }
        return Task.CompletedTask;
    }

    public async Task<bool> Battle()
    {
        behavior.Update();
        return true;
    }

    public Task ToggleAutoSummonMonsterCurrently()
    {
        // NOTE: This function is called each time game controller summons a new monster wave
        Console.WriteLine("ToggleAutoSummonMonsterCurrently");
        hasAutoSummonMonsterCurrently = true;
        Thread.Sleep(2000);
        hasAutoSummonMonsterCurrently = false;
        return Task.CompletedTask;
    }

    public void BotGainEnergy(int energyGain)
    {
        // NOTE: This function is called anytime when bot received energy from anything
        this.energyGain += energyGain;
    }

    public BotPlayMode PlayMode { get { return playMode; } }
    public List<CardModel> CardSelected { get { return cardSelected; } }
    public BotLogicTile[][] TowerBuildingMap { get { return towerBuildingMap; } }
    public int TowerBuildingMapWidth { get { return towerBuildingMapWidth; } }
    public int TowerBuildingMapHeight { get { return towerBuildingMapHeight; } }
    public float EnergyBuildTowerRate { get { return energyBuildTowerRate; } set { energyBuildTowerRate = value; } }
    public BotBT Behavior { get { return behavior; } set { behavior = value; } }

    public float EnergyToBuildTower { get { return energyToBuildTower; } set { energyToBuildTower = value; } }
    public float EnergyToSummonMonster { get { return energyToSummonMonster; } set { energyToSummonMonster = value; } }
    public int EnergyGain { get { return energyGain; } set { energyGain = value; } }
    public Vector2 SpellUsingPosition { get { return spellUsingPosition; } set { spellUsingPosition = value; } }
    public bool HasAutoSummonMonsterCurrently { get { return hasAutoSummonMonsterCurrently; } }
    public Vector2Int? TowerSelectPos { get { return towerSelectPos; } set { towerSelectPos = value; } }
    public FindTowerTypeStrategy FindTowerTypeStrategy { get { return findTowerTypeStrategy; } }
    public FindTowerPosStrategy FindTowerPosStrategy { get { return findTowerPosStrategy; } }
    public List<Vector2Int> TowerBuildOrder { get { return towerBuildOrder; } set { towerBuildOrder = value; } }
    public List<Vector2Int> TowerBuildProgressOrder { get { return towerBuildProgressOrder; } set { towerBuildProgressOrder = value; } }
    public GameSessionModel GameSessionModel { get { return gameSessionModel; } }
    public int RealMapWidth { get { return realMapWidth; } }
    public int RealMapHeight { get {  return realMapHeight; } }
    public string BasicTowerName { get {  return basicTowerName; } }
}
