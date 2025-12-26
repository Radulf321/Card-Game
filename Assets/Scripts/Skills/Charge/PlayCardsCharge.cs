using Newtonsoft.Json.Linq;

public class PlayCardsCharge : SkillCharge
{
    public PlayCardsCharge(JObject json, Skill skill) : base(json, skill)
    {
    }

    protected override void HandleMessage(TriggerMessage triggerMessage)
    {
        if (triggerMessage.GetTriggerType() == TriggerType.PlayCard)
        {
            this.AddProgress(1);
        }
    }
}