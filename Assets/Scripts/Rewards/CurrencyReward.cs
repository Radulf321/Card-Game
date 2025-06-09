using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using UnityEngine;

public class CurrencyReward : Reward
{
    private AmountCalculation amount;
    private string currency;

    public CurrencyReward(AmountCalculation amount, string currency)
    {
        this.amount = amount;
        this.currency = currency;
    }

    public CurrencyReward(int amount, string currency) : this(
        amount: new ConstantAmountCalculation(amount),
        currency: currency)
    {
    }

    public CurrencyReward(JObject rewardData) : this(
        amount: AmountCalculation.FromJson(rewardData["amount"]),
        currency: rewardData["currency"]!.ToString()
    )
    {
    }

    public int GetAmount()
    {
        return this.amount.GetValue();
    }

    public override void Collect()
    {
        Game.Instance.GetPlayer().AddCurrency(this.currency, this.GetAmount());
    }

    public override Task<string> ToNiceString()
    {
        return Task.FromResult(
            Game.Instance.GetCurrencyName(this.currency)
        );
    }

    public override Task<string> GetCaption()
    {
        return Task.FromResult(
            "+" + this.GetAmount() + " " + Game.Instance.GetCurrencyName(this.currency)
        );
    }

    public override Sprite GetSprite()
    {
        return Game.Instance.GetCurrencyIcon(this.currency);
    }
}