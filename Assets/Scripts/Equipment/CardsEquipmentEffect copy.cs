using System.Collections.Generic;
using Newtonsoft.Json.Linq;

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
}