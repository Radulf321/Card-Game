using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using UnityEngine.Localization.Settings;

#nullable enable

public class TotalRequirement : Requirement
{
    private int amount;

    public TotalRequirement(int amount, GoalManager? goalManager = null) : base(goalManager){
        this.amount = amount;
    }

    public TotalRequirement(JObject json, GoalManager? goalManager = null) : this(
        amount: json["amount"]!.ToObject<int>(),
        goalManager: goalManager
    )
    {
    }

    public override bool IsFulfilled() {
        return this.GetGoalManager().GetTotal() >= amount;
    }

    public override Task<string> GetDescription()
    {
        int current = this.GetGoalManager().GetTotal();
        if (current >= amount)
        {
            current = amount; // Cap at max for display
        }
        return AsyncHelper.HandleToTask(
            LocalizationSettings.StringDatabase.GetLocalizedStringAsync("RequirementStrings", "TotalDescription",
            arguments: new Dictionary<string, object> {
                { "current", current },
                { "icon", Game.Instance.GetGoalInlineIcon("total") },
                { "amount", this.amount },
            }
        ));
    }

    public override Requirement Clone()
    {
        return new TotalRequirement(this.amount, this.goalManager);
    }

    public override JObject ToJson()
    {
        return new JObject
        {
            ["type"] = RequirementType.Total.ToString(),
            ["amount"] = this.amount,
        };
    }
}
