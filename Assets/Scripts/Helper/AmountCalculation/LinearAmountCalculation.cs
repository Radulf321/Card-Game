#nullable enable

using Newtonsoft.Json.Linq;
using UnityEngine;

public class LinearAmountCalculation : AmountCalculation {
    private int baseValue;
    private float rate;
    private int? min;
    private int? max;

    public LinearAmountCalculation(int baseValue, float rate, CalculationInput input = CalculationInput.Constant, int? min = null, int? max = null) : base(input)
    {
        this.baseValue = baseValue;
        this.rate = rate;
        this.min = min;
        this.max = max;
    }

    public LinearAmountCalculation(JObject json) : base(json) {
        this.baseValue = json["base"]?.ToObject<int>() ?? 0;
        this.rate = json["rate"]?.ToObject<float>() ?? 0;
        this.min = json["min"]?.ToObject<int>();
        this.max = json["max"]?.ToObject<int>();
    }

    public override int GetValue(int number) {
        int value = Mathf.FloorToInt(baseValue + rate * number);
        if (min.HasValue && value < min.Value) {
            return min.Value;
        }
        if (max.HasValue && value > max.Value) {
            return max.Value;
        }
        return value;
    }
}
