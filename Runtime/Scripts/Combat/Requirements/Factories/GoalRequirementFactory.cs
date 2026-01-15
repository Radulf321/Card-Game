using System;
using Newtonsoft.Json.Linq;
using UnityEngine;

#nullable enable

public class GoalRequirementFactory : RequirementFactory
{
    private string goal;
    private CombatTarget? owner;
    private AmountCalculation amountCalculation;
    private RequirementComparison comparison;
    private float levelIncrement;
    private float variance;

    public GoalRequirementFactory(JObject json, CombatTarget? owner) : base(json)
    {
        this.goal = json["goal"]?.ToString() ?? "Undefined";
        this.amountCalculation = AmountCalculation.FromJson(json["amount"]) ?? new LinearAmountCalculation(baseValue: 3, rate: 1.5f);
        this.levelIncrement = json["levelIncrement"]?.ToObject<float>() ?? 1.2f;
        this.variance = json["variance"]?.ToObject<float>() ?? 0.1f;
        this.comparison = EnumHelper.ParseEnum<RequirementComparison>(json["comparison"]?.ToString()) ?? RequirementComparison.AtLeast;
        this.owner = owner;
    }
    public override Requirement CreateRequirement(int turn)
    {
        float value = amountCalculation.GetRawValue(turn) * (float)Math.Pow(this.levelIncrement, this.owner?.GetLevel() ?? 0) * UnityEngine.Random.Range(1 - variance, 1 + variance);
        return new GoalRequirement(this.goal, Mathf.FloorToInt(value), comparison: this.comparison);
    }
}