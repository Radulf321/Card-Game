#nullable enable

public enum TriggerType
{
    // Game Events
    AddGoal,
    AddProtection,
    AddCurrency,
    StartTurn,
    TalentTree,
    DrawCards,
    EndTurn,
    EndRound,
    EndCombat,
    DiscardCard,
    PlayCard,
    EndDialog,
    Preparation,
    EndGame,
    SkillProgressChanged,

    // UI Events
    CardDragStart,
    CardDragEnd,
}

public class TriggerMessage
{
    private TriggerType type;
    private ContextInfo data;

    public TriggerMessage(TriggerType type, ContextInfo? data = null)
    {
        this.type = type;
        this.data = data ?? new ContextInfo();
    }

    public TriggerType GetTriggerType()
    {
        return this.type;
    }

    public ContextInfo GetData()
    {
        return this.data;
    }
}