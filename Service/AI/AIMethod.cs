using Game_Realtime.Model;
using Game_Realtime.Model.InGame;

namespace Game_Realtime.Service.AI
{
    public static class AIMethod
    {
        public static bool IsBotCardSelectedContain(List<(string, CardType, string, int)> cardSelected, (CardType, string) cardModel)
        {
            foreach (var card in cardSelected)
            {
                if (card.Item2 == cardModel.Item1 && card.Item3 == cardModel.Item2)
                {
                    return true;
                }
            }
            return false;
        }

        public static int GetEnergy(List<(string, CardType, string, int)> cardSelected, (CardType, string) cardModel)
        {
            foreach (var card in cardSelected)
            {
                if (card.Item2 == cardModel.Item1 && card.Item3 == cardModel.Item2)
                {
                    return card.Item4;
                }
            }
            return 0;
        }

        public static int GetMinMonsterEnergy(List<(string, CardType, string, int)> cardSelected)
        {
            int result = 999999999;
            foreach (var card in cardSelected)
            {
                if (card.Item2 == CardType.MonsterCard && card.Item4 < result)
                {
                    result = card.Item4;
                }
            }
            return result;
        }

        public static string GetCardId(List<(string, CardType, string, int)> cardSelected, (CardType, string) cardModel)
        {
            foreach (var card in cardSelected)
            {
                if (card.Item2 == cardModel.Item1 && card.Item3 == cardModel.Item2)
                {
                    return card.Item1;
                }
            }
            return "";
        }
    }
}
