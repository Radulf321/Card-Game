#nullable enable

public enum TriggerType
{
    AddGoal,
}

public class TriggerMessage
{
    private TriggerType type;
    private TriggerMessageData data;

    public TriggerMessage(TriggerType type, TriggerMessageData? data = null)
    {
        this.type = type;
        this.data = data ?? new TriggerMessageData();
    }

    public TriggerType GetTriggerType()
    {
        return this.type;
    }

    public TriggerMessageData GetData()
    {
        return this.data;
    }
}