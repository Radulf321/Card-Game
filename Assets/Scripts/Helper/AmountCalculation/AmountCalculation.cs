using Newtonsoft.Json.Linq;

#nullable enable

public enum CalculationInput
{
    PreviousPlays,
    TargetLevel,
    Constant,
}

public abstract class AmountCalculation
{
    public static AmountCalculation? FromJson(JToken? json)
    {
        if (json == null)
        {
            return null;
        }
        switch (json.Type)
        {
            case JTokenType.Integer:
                return new ConstantAmountCalculation(json.ToObject<int>());

            case JTokenType.Object:
                JObject jsonObject = (JObject)json;
                string type = jsonObject["type"]?.ToString() ?? "";
                switch (type)
                {
                    case "linear":
                        return new LinearAmountCalculation(jsonObject);
                    case "constant":
                        return new ConstantAmountCalculation(jsonObject);
                    default:
                        throw new System.Exception("Invalid amount calculation type: " + type);
                }

            default:
                throw new System.Exception("Invalid amount calculation type: " + json.Type);
        }
    }

    private CalculationInput input;

    public AmountCalculation(CalculationInput input = CalculationInput.Constant)
    {
        this.input = input;
    }

    public AmountCalculation(JObject json)
    {
        this.input = EnumHelper.ParseEnum<CalculationInput>(json["input"]?.ToString()) ?? CalculationInput.Constant;
    }


    public int GetValue(Card? card = null)
    {
        int inputValue;
        switch (input)
        {
            case CalculationInput.Constant:
                inputValue = 0;
                break;

            case CalculationInput.PreviousPlays:
                inputValue = CombatHandler.instance?.getCardsPlayed(card!.GetID()) ?? 0;
                break;

            case CalculationInput.TargetLevel:
                inputValue = Game.Instance.GetCurrentCombatTarget().GetLevel();
                break;

            default:
                throw new System.Exception("Invalid input type: " + input);
        }

        return GetValue(inputValue);
    }

    abstract public int GetValue(int number);
}