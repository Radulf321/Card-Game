using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using UnityEngine.Localization.Settings;

#nullable enable

public class TriggerMessageConditionEndRound : TriggerMessageCondition
{

    public TriggerMessageConditionEndRound()
    {
    }

    public TriggerMessageConditionEndRound(JObject json)
    {
    }

    public override bool FulfillsCondition(TriggerMessage message)
    {
        return (message.GetTriggerType() == TriggerType.EndRound);
    }

    public override Task<string> GetDescription()
    {
        // This trigger is not really meant for cards, so we don't need a description.
        throw new System.NotImplementedException();
    }

    public async override Task<string?> GetIconDescription()
    {
        throw new System.NotImplementedException();
    }
}