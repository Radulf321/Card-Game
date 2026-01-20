#nullable enable

using Newtonsoft.Json.Linq;

public enum CurrencyType {
    Generic,
    Health
}

public class CurrencyData : NamedIconData {
    private int? maximum;
    private int starting;
    private CurrencyType type;
    private string? format;

    public CurrencyData(JObject currencyData) : base(currencyData)
    {
        this.maximum = currencyData["maximum"]?.ToObject<int>();
        this.starting = currencyData["starting"]?.ToObject<int>() ?? 0;
        this.type = EnumHelper.ParseEnum<CurrencyType>(currencyData["type"]?.ToString()) ?? CurrencyType.Generic;
        this.format = LocalizationHelper.GetLocalizedString(currencyData["format"] as JObject);
    }

    public int? GetMaximum()
    {
        return maximum;
    }

    public CurrencyType GetCurrencyType()
    {
        return type;
    }

    public int GetStartingAmount()
    {
        return starting;
    }

    public string GetFormat()
    {
        return format ?? (this.GetInlineIcon() + " ${amount}");
    }
}