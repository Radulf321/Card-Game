using System;
using Newtonsoft.Json.Linq;
using UnityEngine;

#nullable enable

public class TotalRequirementFactory : RequirementFactory
{
    private AmountCalculation amountCalculation;
    private float levelIncrement;
    private float variance;

    public TotalRequirementFactory(JObject json, CombatTarget? owner, Enemy? enemy) : base(json, owner, enemy)
    {
        this.amountCalculation = AmountCalculation.FromJson(json["amount"]) ?? new LinearAmountCalculation(baseValue: 5, rate: 2.5f);
        this.levelIncrement = json["levelIncrement"]?.ToObject<float>() ?? 1.2f;
        this.variance = json["variance"]?.ToObject<float>() ?? 0.1f;
        this.owner = owner;
    }
    public override Requirement CreateRequirement(int turn) {
        float value = this.amountCalculation.GetRawValue(turn) * (float)Math.Pow(this.levelIncrement, this.owner?.GetLevel() ?? 0) * UnityEngine.Random.Range(1 - variance, 1 + variance);
        return new TotalRequirement(Mathf.FloorToInt(value), enemy: this.enemy);
    }
}