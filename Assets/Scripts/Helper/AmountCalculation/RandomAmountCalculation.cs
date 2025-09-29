#nullable enable

using Newtonsoft.Json.Linq;
using UnityEngine;

public class RandomAmountCalculation : AmountCalculation
{
    private float min;
    private float max;

    public RandomAmountCalculation(float min, float max) : base(CalculationInput.Constant)
    {
        this.min = min;
        this.max = max;
    }

    public RandomAmountCalculation(JObject json) : base(json)
    {
        this.min = json["min"]!.ToObject<float>();
        this.max = json["max"]!.ToObject<float>();
    }

    public override float GetRawValue(float number)
    {
        return Random.Range(min, max + 1);
    }
}
