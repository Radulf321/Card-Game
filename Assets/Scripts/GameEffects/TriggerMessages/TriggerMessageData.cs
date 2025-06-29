// Add nullable parameters as needed to fit all types of messages
#nullable enable

public class TriggerMessageData
{
    int? amount;
    string? goal;
    CardEffectTrigger? trigger;

    public TriggerMessageData(int? amount = null, string? goal = null, CardEffectTrigger? trigger = null)
    {
        this.amount = amount;
        this.goal = goal;
        this.trigger = trigger;
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
}