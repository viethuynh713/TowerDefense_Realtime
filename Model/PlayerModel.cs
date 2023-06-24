﻿using System.Numerics;
using Game_Realtime.Model.Data;

namespace Game_Realtime.Model
{
    public class PlayerModel : BasePlayer
    {
        public List<string> cards;

        public List<MonsterModel> monsters;

        public PlayerModel(string id,List<string> cards) : base(id)
        {
            this.cards = cards;

            this.monsters = new List<MonsterModel>();
        }

        public async Task<MonsterModel> CreateMonster(PlaceCardData data)
        {
            var stats = (MonsterStats)(data.stats);
            var monster = new MonsterModel(data.cardId, stats.Hp, new Vector2(data.Xposition, data.Yposition));
            
            monsters.Add(monster);

            return monster;
        }
    }
    public class AiModel: BasePlayer
    {
        public AiModel() : base() { }
    }

    public class BasePlayer
    {
        public string userId;

        public int castleHp;

        public int energy;

        protected BasePlayer() 
        {
            this.userId = Guid.NewGuid().ToString();
            this.castleHp = 1000;
            this.energy = 100;
        }

        protected BasePlayer(string userId)
        {
            this.userId = userId;
            this.castleHp = 1000;
            this.energy = 100;
        }

        public int CastleTakeDamage(int damage)
        {
            this.castleHp -= damage;
            return this.castleHp;
        }

        public void UpdateEnergy(int energy)
        {
            this.energy -= energy;
        }


    }
}
