using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using UnityEngine.Localization.Settings;

#nullable enable

public class CurrencyEffect : GameEffect
{

    private Card? owner;
    private AmountCalculation amountCalculation;
    private string currency;
    private CardEffectTrigger trigger;

    public CurrencyEffect(string currency, AmountCalculation amountCalculation, Card? owner, CardEffectTrigger trigger = CardEffectTrigger.PlayCard)
    {
        this.currency = currency;
        this.amountCalculation = amountCalculation;
        this.owner = owner;
        this.trigger = trigger;
    }

    public CurrencyEffect(string currency, int amount, Card? owner, CardEffectTrigger trigger = CardEffectTrigger.PlayCard) : this(currency, new ConstantAmountCalculation(amount), owner, trigger)
    {
    }

    public CurrencyEffect(JObject json, Card owner, CardEffectTrigger trigger = CardEffectTrigger.PlayCard) : this(json["currency"]?.ToString() ?? "unknown", AmountCalculation.FromJson(json["amount"]) ?? new ConstantAmountCalculation(0), owner, trigger)
    {
    }

    public override void applyEffect(Enemy? target = null)
    {
        Game.Instance.GetPlayer().AddCurrency(currency, this.amountCalculation.GetValue(this.owner));
    }

    public override GameEffect Clone(Card? newOwner)
    {
        return new CurrencyEffect(this.currency, this.amountCalculation, newOwner ?? this.owner, this.trigger);
    }

    private async Task<string> GetDescription(string tableEntry, string? overridePrefix = null, string? overrideSuffix = null)
    {
        return await AsyncHelper.HandleToTask(LocalizationSettings.StringDatabase.GetLocalizedStringAsync("CardStrings", tableEntry,
            arguments: new Dictionary<string, object> {
                { "prefix", overridePrefix ?? (await this.amountCalculation.GetDescriptionPrefix(this.owner)) },
                { "currency", Game.Instance.GetCurrencyInlineIcon(this.currency) },
                { "suffix", overrideSuffix ?? (await this.amountCalculation.GetDescriptionSuffix(this.owner)) }
            }
        ));
    }
    
    public override async Task<string?> GetInternalIconDescription()
    {
        return await GetDescription(
            "CurrencyIcon",
            overridePrefix: await this.amountCalculation.GetDescriptionPrefixIcon(this.owner),
            overrideSuffix: await this.amountCalculation.GetDescriptionSuffixIcon(this.owner)
        );
    }

    public override Task<string> getDescription()
    {
        return GetDescription("Currency");
    }

    public override Task<string> getTriggerDescription()
    {
        return GetDescription("CurrencyTrigger");
    }
}