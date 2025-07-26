// Add nullable parameters as needed to fit all types of messages
#nullable enable

public class TriggerMessageData
{
    private int? amount;
    private string? goal;
    private CardEffectTrigger? trigger;
    private bool? success;
    private DiscardType? discardType;
    private Card? card;

    public TriggerMessageData(int? amount = null, string? goal = null, CardEffectTrigger? trigger = null, bool? success = null, DiscardType? discardType = null, Card? card = null)
    {
        this.amount = amount;
        this.goal = goal;
        this.trigger = trigger;
        this.success = success;
        this.discardType = discardType;
        this.card = card;
    }

    public int? GetAmount()
    {
        return this.amount;
    }

    public string? GetGoal()
    {
        return this.goal;
    }

    public CardEffectTrigger? GetTrigger()
    {
        return this.trigger;
    }

    public bool? GetSuccess()
    {
        return this.success;
    }

    public DiscardType? GetDiscardType()
    {
        return this.discardType;
    }

    public Card? GetCard()
    {
        return this.card;
    }
}