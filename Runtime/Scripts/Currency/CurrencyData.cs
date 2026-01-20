#nullable enable

using Newtonsoft.Json.Linq;
using UnityEngine;

public enum CurrencyType {
    Generic,
    Health
}

public class CurrencyData : NamedIconData {
    private int? maximum;
    private int starting;
    private CurrencyType type;

    public CurrencyData(JObject currencyData) : base(currencyData)
    {
        this.maximum = currencyData["maximum"]?.ToObject<int>();
        this.starting = currencyData["starting"]?.ToObject<int>() ?? 0;
        this.type = EnumHelper.ParseEnum<CurrencyType>(currencyData["type"]?.ToString()) ?? CurrencyType.Generic;
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
}