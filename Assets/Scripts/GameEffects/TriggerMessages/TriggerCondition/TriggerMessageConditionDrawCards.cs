using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using UnityEngine.Localization.Settings;

#nullable enable

public class TriggerMessageConditionDrawCards : TriggerMessageCondition
{
    int minimum;

    public TriggerMessageConditionDrawCards(int minimum = 1)
    {
        this.minimum = minimum;
    }

    public TriggerMessageConditionDrawCards(JObject json) : this(minimum: json["minimum"]?.ToObject<int>() ?? 1)
    {
    }

    public override bool FulfillsCondition(TriggerMessage message)
    {
        if ((message.GetTriggerType() != TriggerType.DrawCards) ||
            (message.GetData().GetTrigger() == CardEffectTrigger.CombatStart) ||
            (message.GetData().GetTrigger() == CardEffectTrigger.TriggerEffect))
        {
            return false;
        }

        return message.GetData().GetAmount() >= this.minimum;
    }

    public override Task<string> GetDescription()
    {
        return AsyncHelper.HandleToTask(LocalizationSettings.StringDatabase.GetLocalizedStringAsync("CardStrings", "TriggerDrawCards",
            arguments: new Dictionary<string, object> {
                { "minimum", this.minimum },
            }
        ));
    }

    public async override Task<string?> GetIconDescription()
    {
        return await AsyncHelper.HandleToTask(LocalizationSettings.StringDatabase.GetLocalizedStringAsync("CardStrings", "DrawCardsIcon",
            arguments: new Dictionary<string, object> {
                { "amount", this.minimum },
            }
        ));
    }
}