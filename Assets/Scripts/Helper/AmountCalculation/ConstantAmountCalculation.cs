using Newtonsoft.Json.Linq;

#nullable enable

public class ConstantAmountCalculation : AmountCalculation
{
    private int value;
    public ConstantAmountCalculation(int value) : base(CalculationInput.Constant)
    {
        this.value = value;
    }

    public ConstantAmountCalculation(JObject json) : base(json)
    {
        this.value = json["value"]?.ToObject<int>() ?? 0;
    }

    public override float GetRawValue(int number)
    {
        return this.value;
    }
}