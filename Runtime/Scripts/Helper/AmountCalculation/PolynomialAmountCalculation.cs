#nullable enable

using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;

public class PolynomialAmountCalculation : AmountCalculation {
    private List<float> coefficients;
    private int? min;
    private int? max;

    public PolynomialAmountCalculation(List<float> coefficients, CalculationInput input = CalculationInput.Constant, int? min = null, int? max = null) : base(input)
    {
        this.coefficients = coefficients;
        this.min = min;
        this.max = max;
    }

    public PolynomialAmountCalculation(JObject json) : base(json) {
        this.coefficients = json["coefficients"]!.ToObject<List<float>>()!;
        this.min = json["min"]?.ToObject<int>();
        this.max = json["max"]?.ToObject<int>();
    }

    public override float GetRawValue(float number) {
        float result = 0;
        for (int i = 0; i < this.coefficients.Count; i++)
        {
            result += this.coefficients[i] * Mathf.Pow(number, i);
        }
        if (min.HasValue && result < min.Value) {
            return min.Value;
        }
        if (max.HasValue && result > max.Value) {
            return max.Value;
        }
        return result;
    }
}
