// Add nullable parameters as needed to fit all types of messages
#nullable enable

public class TriggerMessageData
{
    int? amount;
    string? goal;

    public TriggerMessageData(int? amount = null, string? goal = null)
    {
        this.amount = amount;
        this.goal = goal;
    }

    public int? GetAmount()
    {
        return this.amount;
    }

    public string? GetGoal()
    {
        return this.goal;
    }
}