using System;
using Newtonsoft.Json.Linq;
using UnityEngine;

public class GoalRequirementFactory : RequirementFactory
{
    private string goal;
    private CombatTarget owner;
    private AmountCalculation amountCalculation;

    public GoalRequirementFactory(JObject json, CombatTarget owner) {
        this.goal = json["goal"]?.ToString() ?? "Undefined";
        this.amountCalculation = AmountCalculation.FromJson(json["amount"]) ?? new LinearAmountCalculation(baseValue: 3, rate: 1.5f );
        this.owner = owner;
    }
    public override Requirement CreateRequirement(int turn) {
        float value = amountCalculation.GetValue(turn) * (float)Math.Pow(1.2, this.owner.GetLevel()) * UnityEngine.Random.Range(0.9f, 1.1f);
        return new GoalRequirement(this.goal, Mathf.FloorToInt(value));
    }
}