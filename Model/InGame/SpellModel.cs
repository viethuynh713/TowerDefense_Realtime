namespace Game_Realtime.Model.InGame;

public class SpellModel
{
    public string spellId;
    public string cardId;
    public string ownerId;
    public float XLogicPosition;
    public float YLogicPosition;

    public SpellModel(string cardId,float x,float y, string ownerId)
    {
        this.spellId = Guid.NewGuid().ToString();
        this.cardId = cardId;
        this.XLogicPosition = x;
        this.YLogicPosition = y;
        this.ownerId = ownerId;
    }
}