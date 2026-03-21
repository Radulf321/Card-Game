using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using UnityEngine.Localization.Settings;

#nullable enable

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
    private bool? fulfilled = null;

    public GoalRequirement(string goal, int amount, RequirementComparison comparison = RequirementComparison.AtLeast, GoalManager? goalManager = null) : base(goalManager)
    {
        this.goal = goal;
        this.amount = amount;
        this.comparison = comparison;
    }

    public GoalRequirement(JObject json, GoalManager? goalManager = null) : this(
        goal: json["goal"]!.ToString(),
        amount: json["amount"]!.ToObject<int>(),
        comparison: EnumHelper.ParseEnum<RequirementComparison>(json["comparison"]?.ToString()) ?? RequirementComparison.AtLeast,
        goalManager: goalManager
    )
    {
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
                return this.GetGoalManager().GetGoalAmount(this.goal) >= this.amount;

            case RequirementComparison.Burst:
                bool result = this.GetGoalManager().GetGoalAmountThisTurn(this.goal) >= this.amount;
                if (result)
                {
                    this.fulfilled = true;
                }
                return result;

            case RequirementComparison.Maximum:
                return this.GetGoalManager().GetGoalAmount(this.goal) <= this.amount;

            default:
                throw new NotImplementedException($"Invalid comparison: {comparison}");
        }
    }

    public override Task<string> GetDescription()
    {
        int current;
        GoalManager goalManager = this.GetGoalManager();
        
        switch (comparison)
        {
            case RequirementComparison.AtLeast:
            case RequirementComparison.Maximum:
                current = goalManager.GetGoalAmount(this.goal);
                if (this.comparison == RequirementComparison.AtLeast && (current > this.amount))
                {
                    current = this.amount; // Cap at max for display
                }
                break;

            case RequirementComparison.Burst:
                if (this.fulfilled == true)
                {
                    current = this.amount; // Show as completed
                }
                else
                {
                    current = goalManager.GetGoalAmountThisTurn(this.goal);
                    if (current >= this.amount)
                    {
                        current = this.amount;
                        this.fulfilled = true;
                    }
                }
                break;

            default:
                throw new NotImplementedException($"Invalid comparison: {comparison}");
        }
        int protection = goalManager.GetProtection(this.goal);
        GoalData? goalData = Game.Instance.GetGoalData(this.goal);
        GoalUsageType usageType = GoalUsageType.Default;
        if (goalData == null)
        {
            throw new Exception($"No GoalData found for goal {this.goal}");
        }
        if (goalData.GetDisplayType() == GoalDisplayType.Reversed)
        {
            // For reversed goals, show progress towards 0 instead of towards the amount
            current = Math.Max(0, this.amount - current);
            usageType = GoalUsageType.Reversed;
        }
        return AsyncHelper.HandleToTask(
            LocalizationSettings.StringDatabase.GetLocalizedStringAsync("RequirementStrings", "GoalDescription",
            arguments: new Dictionary<string, object> {
                { "comparison", this.comparison },
                { "icon", goalData.GetInlineIcon(usageType) },
                { "current", current },
                { "amount", this.amount },
                { "protection", protection },
                { "protectionIcon", goalData.GetInlineIcon(GoalUsageType.Protection) ?? "Undefined Protection Icon" },
            }
        ));
    }

    public RequirementComparison GetComparison()
    {
        return this.comparison;
    }

    public override Requirement Clone()
    {
        return new GoalRequirement(this.goal, this.amount, this.comparison, this.goalManager);
    }

    public override JObject ToJson()
    {
        return new JObject
        {
            ["type"] = RequirementType.Goal.ToString(),
            ["goal"] = this.goal,
            ["amount"] = this.amount,
            ["comparison"] = this.comparison.ToString(),
        };
    }
}
