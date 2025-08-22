using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using UnityEngine.Localization.Settings;

#nullable enable

public class CurrencyTask : GameTask
{
    private int amount;
    private string currency;

    private int progress;

    public CurrencyTask(string id, List<Reward> rewards, int amount, string currency) : base(id, rewards)
    {
        this.amount = amount;
        this.currency = currency;
        this.progress = 0;
        Game.Instance.SubscribeToTriggerMessages(this.HandleMessage);
    }

    public CurrencyTask(JObject json) : base(json)
    {
        this.amount = json["amount"]!.ToObject<int>();
        this.currency = json["currency"]!.ToString();
        this.progress = 0;
        Game.Instance.SubscribeToTriggerMessages(this.HandleMessage);
    }

    public override Task<string> GetDescription()
    {
        return AsyncHelper.HandleToTask(LocalizationSettings.StringDatabase.GetLocalizedStringAsync("TaskStrings", "CurrencyDescription",
            arguments: new Dictionary<string, object?> {
                { "amount", this.amount },
                { "currency", Game.Instance.GetCurrencyInlineIcon(this.currency) }
            }
        ));
    }

    public override int? GetProgress()
    {
        return Math.Min(this.progress, this.amount);
    }

    public override int? GetTotal()
    {
        return this.amount;
    }

    private void HandleMessage(TriggerMessage triggerMessage)
    {
        switch (triggerMessage.GetTriggerType())
        {
            case TriggerType.AddCurrency:
                if (((triggerMessage.GetData().GetAmount() ?? 0) > 0) && 
                    (triggerMessage.GetData().GetCurrency() == this.currency))
                {
                    this.progress += triggerMessage.GetData().GetAmount() ?? 0;
                    if (this.progress >= this.amount)
                    {
                        Game.Instance.UnsubscribeFromTriggerMessages(this.HandleMessage);
                    }
                }
                break;

            default:
                break;
        }

    }
}