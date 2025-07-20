using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine.Localization.Settings;

public enum RequirementComparison
{
    AtLeast,
    Maximum
}

public class GoalRequirement : Requirement
{
    private string goal;
    private int amount;
    private RequirementComparison comparison;

    public GoalRequirement(string subject, int amount, RequirementComparison comparison = RequirementComparison.AtLeast)
    {
        this.goal = subject;
        this.amount = amount;
        this.comparison = comparison;
    }

    public override bool IsFulfilled()
    {
        int current = CombatHandler.instance.getGoalAmount(this.goal);
        switch (comparison)
        {
            case RequirementComparison.AtLeast:
                return current >= this.amount;

            case RequirementComparison.Maximum:
                return current <= this.amount;

            default:
                throw new NotImplementedException($"Invalid comparison: {comparison}");
        }
    }

    public override Task<string> GetDescription()
    {
        return AsyncHelper.HandleToTask(
            LocalizationSettings.StringDatabase.GetLocalizedStringAsync("RequirementStrings", "GoalDescription",
            arguments: new Dictionary<string, object> {
                { "comparison", this.comparison },
                { "name", Game.Instance.GetGoalName(this.goal) },
                { "current", CombatHandler.instance.getGoalAmount(this.goal) },
                { "amount", this.amount },
            }
        ));
    }
}
