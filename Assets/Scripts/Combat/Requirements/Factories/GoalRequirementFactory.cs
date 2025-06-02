using System;
using Newtonsoft.Json.Linq;
using UnityEngine;

public class GoalRequirementFactory : RequirementFactory
{
    private string goal;
    private CombatTarget owner;
    private AmountCalculation amountCalculation;
    private float levelIncrement;
    private float variance;

    public GoalRequirementFactory(JObject json, CombatTarget owner) {
        this.goal = json["goal"]?.ToString() ?? "Undefined";
        this.amountCalculation = AmountCalculation.FromJson(json["amount"]) ?? new LinearAmountCalculation(baseValue: 3, rate: 1.5f );
        this.levelIncrement = json["levelIncrement"]?.ToObject<float>() ?? 1.2f;
        this.variance = json["variance"]?.ToObject<float>() ?? 0.1f;
        this.owner = owner;
    }
    public override Requirement CreateRequirement(int turn) {
        float value = amountCalculation.GetValue(turn) * (float)Math.Pow(this.levelIncrement, this.owner.GetLevel()) * UnityEngine.Random.Range(1 - variance, 1 + variance);
        return new GoalRequirement(this.goal, Mathf.FloorToInt(value));
    }
}