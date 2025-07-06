using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

public abstract class EquipmentEffect
{
    public static EquipmentEffect FromJson(JObject effectData)
    {
        string type = effectData["type"]?.ToString() ?? "Undefined";
        return type switch
        {
            "cards" => new CardsEquipmentEffect(effectData),
            "energy" => new EnergyEquipmentEffect(effectData),
            _ => throw new System.Exception($"Unknown equipment effect type: {type}")
        };
    }

    public abstract void ApplyEffect(Player player);
    public abstract Task<string> GetCaption();
}