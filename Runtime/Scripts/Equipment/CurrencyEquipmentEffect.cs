using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using UnityEngine.Localization.Settings;

public class CurrencyEquipmentEffect : EquipmentEffect
{
    private int amount;
    private string currency;
    public CurrencyEquipmentEffect(JObject effectData)
    {
        this.amount = effectData["amount"]?.ToObject<int>() ?? 1;
        this.currency = effectData["currency"]!.ToString();
    }

    public override void ApplyEffect(Player player)
    {
        player.AddCurrency(currency, amount);
    }

    public override Task<string> GetCaption()
    {
        return AsyncHelper.HandleToTask(LocalizationSettings.StringDatabase.GetLocalizedStringAsync("EquipmentStrings", "CurrencyCaption",
            arguments: new Dictionary<string, object> {
                { "amount", this.amount },
                { "currency", Game.Instance.GetCurrencyInlineIcon(this.currency) }
            }
        ));
    }
}