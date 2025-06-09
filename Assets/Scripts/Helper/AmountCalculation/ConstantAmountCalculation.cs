using Newtonsoft.Json.Linq;

#nullable enable

public class ConstantAmountCalculation : AmountCalculation
{
    private int value;
    public ConstantAmountCalculation(int value)
    {
        this.value = value;
    }

    public ConstantAmountCalculation(JObject json)
    {
        this.value = json["value"]?.ToObject<int>() ?? 0;
    }

    public override int GetValue(Card? card = null)
    {
        return this.value;
    }

    public override int GetValue(int number)
    {
        return this.value;
    }
}