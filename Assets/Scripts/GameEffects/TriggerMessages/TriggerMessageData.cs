// Add nullable parameters as needed to fit all types of messages
#nullable enable

public class TriggerMessageData
{
    int? amount;
    string? goal;
    CardEffectTrigger? trigger;
    bool? success;

    public TriggerMessageData(int? amount = null, string? goal = null, CardEffectTrigger? trigger = null, bool? success = null)
    {
        this.amount = amount;
        this.goal = goal;
        this.trigger = trigger;
        this.success = success;
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
}