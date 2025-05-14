using System;
using Newtonsoft.Json.Linq;
using UnityEngine;

public class TotalRequirementFactory : RequirementFactory
{
    private CombatTarget owner;
    private AmountCalculation amountCalculation;

    public TotalRequirementFactory(JObject json, CombatTarget owner) {
        this.amountCalculation = AmountCalculation.FromJson(json["amount"]) ?? new LinearAmountCalculation(baseValue: 5, rate: 2.5f );
        this.owner = owner;
    }
    public override Requirement CreateRequirement(int turn) {
        float value = this.amountCalculation.GetValue(turn) * (float)Math.Pow(1.2, this.owner.GetLevel()) * UnityEngine.Random.Range(0.9f, 1.1f);
        return new TotalRequirement(Mathf.FloorToInt(value));
    }
}