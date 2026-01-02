using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using UnityEngine.Localization.Settings;

public class EnergyEquipmentEffect : EquipmentEffect
{
    private int amount;
    private int turn;
    public EnergyEquipmentEffect(JObject effectData)
    {
        this.amount = effectData["amount"]?.ToObject<int>() ?? 1;
        this.turn = effectData["turn"]?.ToObject<int>() ?? 0;
    }

    public override void ApplyEffect(Player player)
    {
        player.AddEnergy(amount, turn);
    }

    public override Task<string> GetCaption()
    {
        return AsyncHelper.HandleToTask(LocalizationSettings.StringDatabase.GetLocalizedStringAsync("EquipmentStrings", "EnergyCaption",
            arguments: new Dictionary<string, object> {
                { "amount", this.amount },
                { "turn", this.turn }
            }
        ));
    }
}