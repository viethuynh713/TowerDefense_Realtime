using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Service.Models
{
    public class CardModel
    {
        [BsonId]
        [DataMember]
        public MongoDB.Bson.ObjectId _id { get; set; }

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
