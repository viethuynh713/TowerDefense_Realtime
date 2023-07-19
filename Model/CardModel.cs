
using System.ComponentModel.DataAnnotations;


namespace Service.Models
{
    public class CardModel
    {
        public string? CardId { get; set; }

        public string? CardName { get; set; }

        public int Energy { get; set; }

        [Range(0, 5)]
        public int CardStar { get; set; }
        public CardType TypeOfCard { get; set; }
        public RarityCard CardRarity { get; set; }
        // public int CardPrice { get; set; }
    }
    public enum CardType
    {
        None,
        TowerCard,
        MonsterCard,
        SpellCard
    }
    public enum RarityCard
    {
        None,
        Common,
        Rare,
        Mythic,
        Legend
    }
    public enum GachaType
    {
        None,
        Common,
        Rare,
        Legend
    }
}
