using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using UnityEngine.Localization.Settings;

#nullable enable

public class TriggerMessageConditionEndTurn : TriggerMessageCondition
{
    int? number;

    public TriggerMessageConditionEndTurn(int? number = null)
    {
        this.number = number;
    }

    public TriggerMessageConditionEndTurn(JObject json) : this(number: json["number"]?.ToObject<int>())
    {
    }

    public override bool FulfillsCondition(TriggerMessage message)
    {
        if (message.GetTriggerType() != TriggerType.EndTurn)
        {
            return false;
        }

        return (this.number == null) || (message.GetData().GetAmount() == this.number);
    }

    public override Task<string> GetDescription()
    {
        return AsyncHelper.HandleToTask(LocalizationSettings.StringDatabase.GetLocalizedStringAsync("CardStrings", "TriggerEndTurn",
            arguments: new Dictionary<string, object?> {
                { "number", (this.number == null) ? null : (this.number + 1) },
            }
        ));
    }

    public async override Task<string?> GetIconDescription()
    {
        return await AsyncHelper.HandleToTask(LocalizationSettings.StringDatabase.GetLocalizedStringAsync("CardStrings", "TriggerEndTurnIcon",
            arguments: new Dictionary<string, object?> {
                { "number", (this.number == null) ? null : (this.number + 1) },
            }
        ));
    }
}