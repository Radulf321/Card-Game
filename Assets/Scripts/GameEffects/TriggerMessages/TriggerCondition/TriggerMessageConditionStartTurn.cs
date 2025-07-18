using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using UnityEngine.Localization.Settings;

#nullable enable

public class TriggerMessageConditionStartTurn : TriggerMessageCondition
{
    int? number;

    public TriggerMessageConditionStartTurn(int? number = null)
    {
        this.number = number;
    }

    public TriggerMessageConditionStartTurn(JObject json) : this(number: json["number"]?.ToObject<int>())
    {
    }

    public override bool FulfillsCondition(TriggerMessage message)
    {
        if (message.GetTriggerType() != TriggerType.StartTurn)
        {
            return false;
        }

        return (this.number == null) || (message.GetData().GetAmount() == this.number);
    }

    public override Task<string> GetDescription()
    {
        return AsyncHelper.HandleToTask(LocalizationSettings.StringDatabase.GetLocalizedStringAsync("CardStrings", "TriggerStartTurn",
            arguments: new Dictionary<string, object> {
                { "number", this.number },
            }
        ));
    }
}