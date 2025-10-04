using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine.Localization.Settings;

public enum RequirementComparison
{
    AtLeast,
    Maximum,
    Burst,
}

public class GoalRequirement : Requirement
{
    private string goal;
    private int amount;
    private RequirementComparison comparison;
    bool? fulfilled = null;

    public GoalRequirement(string goal, int amount, RequirementComparison comparison = RequirementComparison.AtLeast)
    {
        this.goal = goal;
        this.amount = amount;
        this.comparison = comparison;
    }

    public override bool IsFulfilled()
    {
        if (this.fulfilled.HasValue)
        {
            return this.fulfilled.Value;
        }
        switch (comparison)
        {
            case RequirementComparison.AtLeast:
                return CombatHandler.instance.getGoalAmount(this.goal) >= this.amount;

            case RequirementComparison.Burst:
                bool result = CombatHandler.instance.getGoalAmountThisTurn(this.goal) >= this.amount;
                if (result)
                {
                    this.fulfilled = true;
                }
                return result;

            case RequirementComparison.Maximum:
                return CombatHandler.instance.getGoalAmount(this.goal) <= this.amount;

            default:
                throw new NotImplementedException($"Invalid comparison: {comparison}");
        }
    }

    public override Task<string> GetDescription()
    {
        int current;
        
        switch (comparison)
        {
            case RequirementComparison.AtLeast:
            case RequirementComparison.Maximum:
                current = CombatHandler.instance.getGoalAmount(this.goal);
                break;

            case RequirementComparison.Burst:
                if (this.fulfilled == true)
                {
                    current = this.amount; // Show as completed
                }
                else
                {
                    current = CombatHandler.instance.getGoalAmountThisTurn(this.goal);
                    if (current >= this.amount)
                    {
                        this.fulfilled = true;
                    }
                }
                break;

            default:
                throw new NotImplementedException($"Invalid comparison: {comparison}");
        }
        return AsyncHelper.HandleToTask(
            LocalizationSettings.StringDatabase.GetLocalizedStringAsync("RequirementStrings", "GoalDescription",
            arguments: new Dictionary<string, object> {
                { "comparison", this.comparison },
                { "icon", Game.Instance.GetGoalInlineIcon(this.goal) },
                { "current", current },
                { "amount", this.amount },
            }
        ));
    }

    public RequirementComparison GetComparison()
    {
        return this.comparison;
    }
}
