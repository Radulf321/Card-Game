using Newtonsoft.Json.Linq;
using UnityEngine;

#nullable enable

public class LinearAmountCalculation : AmountCalculation {
    private int baseValue;
    private float rate;
    private int? min;
    private string? input;

    public LinearAmountCalculation(int baseValue, float rate, string? input = null, int? min = null) {
        this.baseValue = baseValue;
        this.rate = rate;
        this.input = input;
        this.min = min;
    }

    public LinearAmountCalculation(JObject json) {
        this.baseValue = json["base"]?.ToObject<int>() ?? 0;
        this.rate = json["rate"]?.ToObject<float>() ?? 0;
        this.input = json["input"]?.ToString() ?? "";
        this.min = json["min"]?.ToObject<int>();
    }

    public override int GetValue(Card card) {
        int inputValue;
        switch (input) {
            case "previousPlays":
                inputValue = CombatHandler.instance.getCardsPlayed(card.GetID());
                break;

            default:
                throw new System.Exception("Invalid input type: " + input);
        }

        return GetValue(inputValue);
    }

    public override int GetValue(int number) {
        int value = Mathf.FloorToInt(baseValue + rate * number);
        if (min.HasValue && value < min.Value) {
            return min.Value;
        }
        return value;
    }
}