#nullable enable

using System;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using UnityEngine.Localization.Settings;

public class LinearAmountCalculation : AmountCalculation
{
    private float baseValue;
    private float rate;
    private int? min;
    private int? max;

    public LinearAmountCalculation(float baseValue, float rate, CalculationInput input = CalculationInput.Constant, int? min = null, int? max = null) : base(input)
    {
        this.baseValue = baseValue;
        this.rate = rate;
        this.min = min;
        this.max = max;
    }

    public LinearAmountCalculation(JObject json) : base(json)
    {
        this.baseValue = json["base"]?.ToObject<float>() ?? 0;
        this.rate = json["rate"]?.ToObject<float>() ?? 0;
        this.min = json["min"]?.ToObject<int>();
        this.max = json["max"]?.ToObject<int>();
    }

    public override float GetRawValue(float number)
    {
        float value = baseValue + rate * number;
        if (min.HasValue && value < min.Value)
        {
            return min.Value;
        }
        if (max.HasValue && value > max.Value)
        {
            return max.Value;
        }
        return value;
    }

    public override Task<string> GetDescriptionPrefix(Card? card = null)
    {
        if (ShowCustomPrefixSuffix())
        {
            return Task.FromResult(rate.ToString("0.##"));
        }
        else
        {
            return base.GetDescriptionPrefix(card);
        }
    }

    public override Task<string> GetDescriptionSuffix(Card? card = null)
    {
        if (ShowCustomPrefixSuffix())
        {
            switch (input)
            {
                case CalculationInput.TargetLevel:
                    return AsyncHelper.HandleToTask(LocalizationSettings.StringDatabase.GetLocalizedStringAsync("CardStrings", "AmountCalculationTargetLevel"));

                case CalculationInput.PreviousPlays:
                    return AsyncHelper.HandleToTask(LocalizationSettings.StringDatabase.GetLocalizedStringAsync("CardStrings", "AmountCalculationPreviousPlays"));

                case CalculationInput.CurrentEnergy:
                    return AsyncHelper.HandleToTask(LocalizationSettings.StringDatabase.GetLocalizedStringAsync("CardStrings", "AmountCalculationCurrentEnergy"));

                case CalculationInput.TotalLevels:
                    return AsyncHelper.HandleToTask(LocalizationSettings.StringDatabase.GetLocalizedStringAsync("CardStrings", "AmountCalculationTotalLevels"));

                case CalculationInput.CardsInHand:
                    return AsyncHelper.HandleToTask(LocalizationSettings.StringDatabase.GetLocalizedStringAsync("CardStrings", "AmountCalculationCardsInHand"));

                default:
                    throw new Exception("Invalid input type: " + input);
            }
        }
        else
        {
            return base.GetDescriptionSuffix(card);
        }
    }

    public override Task<string> GetDescriptionSuffixIcon(Card? card = null)
    {
        if (ShowCustomPrefixSuffix())
        {
            switch (input)
            {
                case CalculationInput.TargetLevel:
                case CalculationInput.PreviousPlays:
                case CalculationInput.TotalLevels:
                    return GetDescriptionSuffix(card);

                case CalculationInput.CurrentEnergy:
                    return Task.FromResult("/<sprite name=\"Energy\">");

                case CalculationInput.CardsInHand:
                    return Task.FromResult("/<sprite name=\"Card\">");

                default:
                    throw new Exception("Invalid input type: " + input);
            }
        }
        else
        {
            return base.GetDescriptionSuffixIcon(card);
        }
    }
    
    private bool ShowCustomPrefixSuffix()
    {
        return (min == null) && (max == null) && (baseValue == 0);
    }
}
