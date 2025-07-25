using Newtonsoft.Json.Linq;
using UnityEngine;

#nullable enable

public enum CalculationInput
{
    PreviousPlays,
    TargetLevel,
    Constant,
    CurrentEnergy,
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
                    case "random":
                        return new RandomAmountCalculation(jsonObject);
                    case "polynom":
                        return new PolynomialAmountCalculation(jsonObject);
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

            case CalculationInput.CurrentEnergy:
                inputValue = CombatHandler.instance?.getCurrentEnergy() ?? Game.Instance.GetPlayer().GetStartingEnergy();
                break;

            default:
                throw new System.Exception("Invalid input type: " + input);
        }

        return GetValue(inputValue);
    }

    public int GetValue(int number)
    {
        return Mathf.FloorToInt(GetRawValue(number));
    }

    abstract public float GetRawValue(int number);

}