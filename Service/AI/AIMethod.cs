using Game_Realtime.Model;
using Service.Models;

namespace Game_Realtime.Service.AI
{
    public static class AIMethod
    {
        public static CardModel GetCardModel(List<CardModel> cardSelected, (CardType, string) sampleCard)
        {
            foreach (var card in cardSelected)
            {
                if (card.TypeOfCard == sampleCard.Item1 && card.CardName == sampleCard.Item2)
                {
                    return card;
                }
            }
            return new CardModel();
        }
        public static bool IsBotCardSelectedContain(List<CardModel> cardSelected, (CardType, string) sampleCard)
        {
            foreach (var card in cardSelected)
            {
                if (card.TypeOfCard == sampleCard.Item1 && card.CardName == sampleCard.Item2)
                {
                    return true;
                }
            }
            return false;
        }

        public static int GetEnergy(List<CardModel> cardSelected, (CardType, string) sampleCard)
        {
            foreach (var card in cardSelected)
            {
                if (card.TypeOfCard == sampleCard.Item1 && card.CardName == sampleCard.Item2)
                {
                    return card.Energy;
                }
            }
            return 0;
        }

        public static int GetEnergy(List<CardModel> cardSelected, string cardId)
        {
            foreach (var card in cardSelected)
            {
                if (card.CardId == cardId)
                {
                    return card.Energy;
                }
            }
            return 0;
        }

        public static int GetMinMonsterEnergy(List<CardModel> cardSelected)
        {
            int result = 999999999;
            foreach (var card in cardSelected)
            {
                if (card.TypeOfCard == CardType.MonsterCard && card.Energy < result)
                {
                    result = card.Energy;
                }
            }
            return result;
        }

        public static string GetCardId(List<CardModel> cardSelected, (CardType, string) sample)
        {
            foreach (var card in cardSelected)
            {
                if (card.TypeOfCard == sample.Item1 && card.CardName == sample.Item2)
                {
                    return card.CardId != null ? card.CardId : "";
                }
            }
            return "";
        }
    }
}
