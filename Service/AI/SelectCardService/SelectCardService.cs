using Game_Realtime.Model.InGame;
using Game_Realtime.Service.AI;
namespace Game_Realtime.Service.AI.SelectCardService;

public static class SelectCardService
{
    private class CardPower
    {
        public CardType CardType { get; set; }
        public string Id { get; set; }
        public BotPlayMode BotPlayMode { get; set; }
        public static Dictionary<CardType, string[]> idComparation;
        public CardPower(CardType cardType, string id, BotPlayMode botPlayMode)
        {
            CardType = cardType;
            Id = id;
            BotPlayMode = botPlayMode;

            idComparation = new Dictionary<CardType, string[]>() {
                { CardType.TowerCard, new string[] { } },
                { CardType.MonsterCard, new string[] { } },
                { CardType.SpellCard, new string[] { } }
            };
            foreach (var comparation in idComparation)
            {
                if (AIConstant.CardGroup.TryGetValue(comparation.Key, out var cardGroup))
                {
                    foreach (var tinyCardGroup in cardGroup)
                    {
                        if (idComparation.TryGetValue(comparation.Key, out var _comparation))
                        {
                            var temp = _comparation;
                            Array.Resize(ref _comparation, _comparation.Length + tinyCardGroup.Value.Length);
                            tinyCardGroup.Value.CopyTo(_comparation, temp.Length);
                        }
                    }
                }
            }
        }
        public static bool operator > (CardPower left, CardPower right)
        {
            if (left.BotPlayMode != right.BotPlayMode) return false;
            CardType[] cardTypeComparation = new CardType[] { };
            switch (left.BotPlayMode)
            {
                case BotPlayMode.ATTACK:
                    cardTypeComparation = new CardType[] { CardType.TowerCard, CardType.SpellCard, CardType.MonsterCard };
                    break;
                case BotPlayMode.DEFEND:
                    cardTypeComparation = new CardType[] { CardType.MonsterCard, CardType.SpellCard, CardType.TowerCard };
                    break;
                case BotPlayMode.HYBRIC:
                    cardTypeComparation = new CardType[] { CardType.SpellCard, CardType.MonsterCard, CardType.TowerCard };
                    break;
            }

            int leftCardTypeOrder = Array.IndexOf(cardTypeComparation, left.CardType);
            int rightCardTypeOrder = Array.IndexOf(cardTypeComparation, right.CardType);
            if (leftCardTypeOrder > rightCardTypeOrder) return true;
            if (leftCardTypeOrder < rightCardTypeOrder) return false;
            
            if (idComparation.TryGetValue(left.CardType, out var compartion))
            {
                int leftIdOrder = Array.IndexOf(compartion, left.Id);
                int rightIdOrder = Array.IndexOf(compartion, right.Id);
                if (leftIdOrder >= rightIdOrder) return true;
            }
            return false;
        }

        public static bool operator <(CardPower left, CardPower right)
        {
            if (left.BotPlayMode != right.BotPlayMode) return false;
            CardType[] cardTypeComparation = new CardType[] { };
            switch (left.BotPlayMode)
            {
                case BotPlayMode.ATTACK:
                    cardTypeComparation = new CardType[] { CardType.TowerCard, CardType.SpellCard, CardType.MonsterCard };
                    break;
            }

            int leftCardTypeOrder = Array.IndexOf(cardTypeComparation, left.CardType);
            int rightCardTypeOrder = Array.IndexOf(cardTypeComparation, right.CardType);
            if (leftCardTypeOrder < rightCardTypeOrder) return true;
            if (leftCardTypeOrder > rightCardTypeOrder) return false;

            if (idComparation.TryGetValue(left.CardType, out var compartion))
            {
                int leftIdOrder = Array.IndexOf(compartion, left.Id);
                int rightIdOrder = Array.IndexOf(compartion, right.Id);
                if (leftIdOrder <= rightIdOrder) return true;
            }
            return false;
        }
    }

    public static bool IsGreaterThan((string, CardType, string, int) x, (string, CardType, string, int) y, BotPlayMode mode)
    {
        CardPower cardPowerX = new CardPower(x.Item2, x.Item3, mode);
        CardPower cardPowerY = new CardPower(y.Item2, y.Item3, mode);
        return cardPowerX > cardPowerY;
    }
}