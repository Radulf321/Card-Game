using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using UnityEngine.Localization.Settings;

public class PlayCardsCharge : SkillCharge
{
    public PlayCardsCharge(JObject json, Skill skill) : base(json, skill)
    {
    }

    public override Task<string> GetTextDescription()
    {
        return AsyncHelper.HandleToTask(LocalizationSettings.StringDatabase.GetLocalizedStringAsync("ChargeStrings", "PlayCards"        ));
    }

    protected override void HandleMessage(TriggerMessage triggerMessage)
    {
        if (triggerMessage.GetTriggerType() == TriggerType.PlayCard)
        {
            this.AddProgress(1);
        }
    }
}