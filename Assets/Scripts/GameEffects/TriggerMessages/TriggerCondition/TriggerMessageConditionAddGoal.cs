using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using UnityEngine.Localization.Settings;

#nullable enable

public class TriggerMessageConditionAddGoal : TriggerMessageCondition
{
    string? goal;
    int min;

    public TriggerMessageConditionAddGoal(string? goal = null, int min = 1)
    {
        this.goal = goal;
        this.min = min;
    }

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

    public override Task<string> GetDescription()
    {
        return AsyncHelper.HandleToTask(LocalizationSettings.StringDatabase.GetLocalizedStringAsync("CardStrings", "TriggerAddGoal",
            arguments: new Dictionary<string, object> {
                { "min", this.min },
                { "goal", Game.Instance.GetGoalInlineIcon(this.goal ?? "unknown")}
            }
        ));
    }

    public async override Task<string?> GetIconDescription()
    {
        return await AsyncHelper.HandleToTask(LocalizationSettings.StringDatabase.GetLocalizedStringAsync("CardStrings", "GoalIcon",
            arguments: new Dictionary<string, object> {
                { "amount", this.min },
                { "goal", Game.Instance.GetGoalInlineIcon(this.goal ?? "unknown")}
            }
        ));
    }
}