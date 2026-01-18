using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine.Localization.Settings;

#nullable enable

public class TotalRequirement : Requirement
{
    private int amount;

    public TotalRequirement(int amount, Enemy? enemy = null) : base(enemy){
        this.amount = amount;
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
        return new TotalRequirement(this.amount, this.enemy);
    }
}
