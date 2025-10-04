#nullable enable

using Newtonsoft.Json.Linq;
using UnityEngine;

public class RandomAmountCalculation : AmountCalculation
{
    private AmountCalculation min;
    private AmountCalculation max;

    public RandomAmountCalculation(float min, float max) : base(CalculationInput.Constant)
    {
        this.min = new ConstantAmountCalculation(min);
        this.max = new ConstantAmountCalculation(max);
    }

    public RandomAmountCalculation(JObject json) : base(json)
    {
        this.min = AmountCalculation.FromJson(json["min"])!;
        this.max = AmountCalculation.FromJson(json["max"])!;
    }

    public override float GetRawValue(float number)
    {
        float max = this.max.GetRawValue(number);
        if (max % 1 == 0)
        {
            max += 0.9999f; // Upper bound has the full probability of being chosen
        }

        return Random.Range(min.GetRawValue(number), max);
    }
}
