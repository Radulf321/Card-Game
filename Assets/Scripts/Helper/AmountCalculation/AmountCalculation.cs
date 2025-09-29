using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using UnityEngine;

#nullable enable

public enum CalculationInput
{
    PreviousPlays,
    TargetLevel,
    Constant,
    CurrentEnergy,
    TotalLevels,
    CardsInHand,
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

    protected CalculationInput input;

    public AmountCalculation(CalculationInput input = CalculationInput.Constant)
    {
        this.input = input;
    }

    public AmountCalculation(JObject json)
    {
        this.input = EnumHelper.ParseEnum<CalculationInput>(json["input"]?.ToString()) ?? CalculationInput.Constant;
    }

    public float GetRawValue(Card? card = null)
    {
        float inputValue;
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

            case CalculationInput.TotalLevels:
                inputValue = 0;
                foreach (CombatTarget combatTarget in Game.Instance.GetCharacterManager().GetAvailableCombatTargets())
                {
                    inputValue += combatTarget.GetLevel();
                }
                break;

            case CalculationInput.CardsInHand:
                List<Card> hand = CombatHandler.instance?.getCardPile().GetHand() ?? new List<Card>();
                inputValue = hand.Count;
                if ((card != null) && hand.Contains(card))
                {
                    inputValue -= 1;
                }
                break;

            default:
                throw new System.Exception("Invalid input type: " + input);
        }

        return GetRawValue(inputValue);
    }


    public int GetValue(Card? card = null)
    {
        return Mathf.FloorToInt(GetRawValue(card));
    }

    public virtual Task<string> GetDescriptionPrefix(Card? card = null)
    {
        return Task.FromResult(GetValue(card).ToString());
    }

    public virtual Task<string> GetDescriptionSuffix(Card? card = null)
    {
        return Task.FromResult("");
    }

    public virtual Task<string> GetDescriptionPrefixIcon(Card? card = null)
    {
        return GetDescriptionPrefix(card);
    }

    public virtual Task<string> GetDescriptionSuffixIcon(Card? card = null)
    {
        return GetDescriptionSuffix(card);
    }

    public int GetValue(int number)
    {
        return Mathf.FloorToInt(GetRawValue(number));
    }

    abstract public float GetRawValue(float number);

}