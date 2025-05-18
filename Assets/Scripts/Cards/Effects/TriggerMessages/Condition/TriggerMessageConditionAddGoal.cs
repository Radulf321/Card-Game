using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine.Localization.Settings;

#nullable enable

public class TriggerMessageConditionAddGoal : TriggerMessageCondition
{
    string? goal;
    int min;

    public TriggerMessageConditionAddGoal(JObject json)
    {
        this.goal = json["goal"]?.ToString();
        this.min = json["min"]?.ToObject<int>() ?? 1;
    }

    public override bool FulfillsCondition(TriggerMessage message)
    {
        if (message.GetTriggerType() != TriggerType.AddGoal)
        {
            return false;
        }

        TriggerMessageData data = message.GetData();
        return ((this.goal == null) || (this.goal == data.GetGoal())) && (data.GetAmount() >= this.min);
    }

    public override string GetDescription()
    {
        return LocalizationSettings.StringDatabase.GetLocalizedString("CardStrings", "ConditionAddGoal",
            arguments: new Dictionary<string, object> {
                { "min", this.min },
                { "goal", Game.Instance.GetGoalName(this.goal ?? "unknown")}
            }
        );
    }
}