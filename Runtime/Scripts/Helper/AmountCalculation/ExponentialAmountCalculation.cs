#nullable enable

using Newtonsoft.Json.Linq;
using UnityEngine;

public class ExponentialAmountCalculation : AmountCalculation
{
    private float baseValue;

    public ExponentialAmountCalculation(float baseValue, CalculationInput input = CalculationInput.Constant) : base(input)
    {
        this.baseValue = baseValue;
    }

    public ExponentialAmountCalculation(JObject json) : base(json)
    {
        this.baseValue = json["base"]?.ToObject<float>() ?? 2.0f;
    }

    public override float GetRawValue(float number)
    {
        return Mathf.Pow(baseValue, number);
    }
}