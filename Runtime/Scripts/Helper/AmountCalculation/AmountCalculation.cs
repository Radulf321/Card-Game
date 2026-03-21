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
    Calculation,
    Flag,
    GoalCurrentTurn,
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
                    case "exponential":
                        return new ExponentialAmountCalculation(jsonObject);
                    default:
                        throw new System.Exception("Invalid amount calculation type: " + type);
                }

            default:
                throw new System.Exception("Invalid amount calculation type: " + json.Type + " - " + json.ToString());
        }
    }

    protected CalculationInput input;
    protected string? flag;
    protected AmountCalculation? calculation;
    protected string? goal;

    public AmountCalculation(CalculationInput input = CalculationInput.Constant, string? flag = null, AmountCalculation? calculation = null, string? goal = null)
    {
        this.input = input;
        this.flag = flag;
        this.calculation = calculation;
        this.goal = goal;
    }

    public AmountCalculation(JObject json)
    {
        this.input = EnumHelper.ParseEnum<CalculationInput>(json["input"]?.ToString()) ?? CalculationInput.Constant;
        this.flag = json["flag"]?.ToString();
        this.calculation = FromJson(json["calculation"]);
        this.goal = json["goal"]?.ToString();
    }

    public float GetRawValue(IEffectOwner? owner = null)
    {
        float inputValue;
        switch (input)
        {
            case CalculationInput.Constant:
                inputValue = 0;
                break;

            case CalculationInput.PreviousPlays: {
                if (owner is Card card)
                {
                    inputValue = CombatHandler.instance?.getCardsPlayed(card.GetID()) ?? 0;
                }
                else
                {
                    throw new System.Exception("PreviousPlays input requires a card owner");
                }
                break;
            }

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

            case CalculationInput.CardsInHand: {
                List<Card> hand = CombatHandler.instance?.getCardPile().GetHand() ?? new List<Card>();
                inputValue = hand.Count;
                if (owner is Card card && hand.Contains(card))
                {
                    inputValue -= 1;
                }
                break;
            }

            case CalculationInput.Calculation:
                if (this.calculation == null)
                {
                    throw new System.Exception("Calculation input requires a calculation");
                }
                inputValue = this.calculation.GetRawValue(owner);
                break;

            case CalculationInput.Flag:
                if (this.flag == null)
                {
                    throw new System.Exception("Flag input requires a flag");
                }
                inputValue = Game.Instance.GetFlag(null, this.flag) switch
                {
                    int intValue => intValue,
                    float floatValue => floatValue,
                    _ => 0
                };
                break;

            case CalculationInput.GoalCurrentTurn:
                if (this.goal == null)
                {
                    throw new System.Exception("GoalCurrentTurn input requires a goal");
                }
                inputValue = CombatHandler.instance?.getGoalAmountThisTurn(this.goal) ?? 0;
                break;

            default:
                throw new System.Exception("Invalid input type: " + input);
        }

        return GetRawValue(inputValue);
    }


    public int GetValue(IEffectOwner? owner = null)
    {
        return Mathf.FloorToInt(GetRawValue(owner));
    }

    public virtual Task<string> GetDescriptionPrefix(IEffectOwner? owner = null, bool negate = false)
    {
        int value = GetValue(owner);
        if (negate)
        {
            value = -value;
        }
        return Task.FromResult(value.ToString());
    }

    public virtual Task<string> GetDescriptionSuffix(IEffectOwner? owner = null)
    {
        return Task.FromResult("");
    }

    public virtual Task<string> GetDescriptionPrefixIcon(IEffectOwner? owner = null, bool negate = false)
    {
        return GetDescriptionPrefix(owner, negate);
    }

    public virtual Task<string> GetDescriptionSuffixIcon(IEffectOwner? owner = null)
    {
        return GetDescriptionSuffix(owner);
    }

    public int GetValue(int number)
    {
        return Mathf.FloorToInt(GetRawValue(number));
    }

    abstract public float GetRawValue(float number);

}