using Newtonsoft.Json.Linq;

public class PlayCardsCharge : SkillCharge
{
    public PlayCardsCharge(JObject json) : base(json)
    {
    }

    protected override void HandleMessage(TriggerMessage triggerMessage)
    {
        if (triggerMessage.GetTriggerType() == TriggerType.PlayCard)
        {
            this.progress++;
        }
    }
}