using Game_Realtime.Model;
using Game_Realtime.Model.InGame;

namespace Game_Realtime.Service.AI
{
    public static class AIMethod
    {
        public static bool IsBotCardSelectedContain(List<(CardType, string, int)> cardSelected, (CardType, string) cardModel)
        {
            foreach (var card in cardSelected)
            {
                if (card.Item1 == cardModel.Item1 && card.Item2 == cardModel.Item2)
                {
                    return true;
                }
            }
            return false;
        }

        public static int GetEnergy(List<(CardType, string, int)> cardSelected, (CardType, string) cardModel)
        {
            foreach (var card in cardSelected)
            {
                if (card.Item1 == cardModel.Item1 && card.Item2 == cardModel.Item2)
                {
                    return card.Item3;
                }
            }
            return 0;
        }

        public static int GetMinMonsterEnergy(List<(CardType, string, int)> cardSelected)
        {
            int result = 999999999;
            foreach (var card in cardSelected)
            {
                if (card.Item1 == CardType.MonsterCard && card.Item3 < result)
                {
                    result = card.Item3;
                }
            }
            return result;
        }
    }
}
