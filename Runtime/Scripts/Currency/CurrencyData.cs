#nullable enable

using System.Collections.Generic;
using Newtonsoft.Json.Linq;

public enum CurrencyType {
    Generic,
    Health
}

public enum CurrencyContext
{
    Dialog,
    Combat,
}

public class CurrencyData : NamedIconData {
    private int? maximum;
    private int starting;
    private CurrencyType type;
    private string? format;
    private List<CurrencyContext> contexts;


    public CurrencyData(JObject currencyData) : base(currencyData)
    {
        this.maximum = currencyData["maximum"]?.ToObject<int>();
        this.starting = currencyData["starting"]?.ToObject<int>() ?? 0;
        this.type = EnumHelper.ParseEnum<CurrencyType>(currencyData["type"]?.ToString()) ?? CurrencyType.Generic;
        this.format = LocalizationHelper.GetLocalizedString(currencyData["format"] as JObject);
        if (currencyData["contexts"] == null)
        {
            this.contexts = new List<CurrencyContext>() { CurrencyContext.Dialog };
        }
        else
        {
            List<CurrencyContext> contexts = new List<CurrencyContext>();
            foreach (JToken contextToken in currencyData["contexts"]!)
            {
                CurrencyContext? context = EnumHelper.ParseEnum<CurrencyContext>(contextToken.ToString());
                if (context != null)
                {
                    contexts.Add(context.Value);
                }
            }
            this.contexts = contexts;
        }
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
        if (this.format != null)
        {
            return this.format;
        }
        else if (GetMaximum() != null)
        {
            return this.GetInlineIcon() + " ${amount} / " + GetMaximum();
        }
        else
        {
            return this.GetInlineIcon() + " ${amount}";
        }
    }

    public List<CurrencyContext> GetContexts()
    {
        return contexts;
    }

    public bool HasContext(CurrencyContext context)
    {
        return contexts.Contains(context);
    }
}