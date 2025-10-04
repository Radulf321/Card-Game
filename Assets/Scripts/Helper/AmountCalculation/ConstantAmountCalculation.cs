using Newtonsoft.Json.Linq;

#nullable enable

public class ConstantAmountCalculation : AmountCalculation
{
    private float value;
    public ConstantAmountCalculation(float value) : base(CalculationInput.Constant)
    {
        this.value = value;
    }

    public ConstantAmountCalculation(JObject json) : base(json)
    {
        this.value = json["value"]?.ToObject<float>() ?? 0;
    }

    public override float GetRawValue(float number)
    {
        return this.value;
    }
}