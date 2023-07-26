using Game_Realtime.Model;
using Service.Models;

namespace Game_Realtime.Service.AI
{
    public static class AIMethod
    {
        public static CardModel? GetCardModel(List<CardModel> cardSelected, (CardType, string) sampleCard)
        {
            foreach (var card in cardSelected)
            {
                if (card.TypeOfCard == sampleCard.Item1 && card.CardName == sampleCard.Item2)
                {
                    return card;
                }
            }
            return null;
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
    }
}
