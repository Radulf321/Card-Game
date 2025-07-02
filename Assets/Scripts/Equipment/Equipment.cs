using System.Collections.Generic;
using Newtonsoft.Json.Linq;

public class Equipment
{
    private string id;
    private string name;
    private string iconPath;
    private string slot;
    private List<EquipmentEffect> effects = new List<EquipmentEffect>();

    public Equipment(string id, string name, string iconPath, List<EquipmentEffect> effects, string slot)
    {
        this.slot = slot;
        this.id = id;
        this.name = name;
        this.iconPath = iconPath;
        this.effects = effects;
    }

    public Equipment(JObject equipmentData)
    {
        this.id = equipmentData["id"].ToString();
        this.name = LocalizationHelper.GetLocalizedString(equipmentData["name"] as JObject)!;
        this.iconPath = equipmentData["icon"]!.ToString();
        this.slot = equipmentData["slot"]!.ToString();
        List<EquipmentEffect> effects = new List<EquipmentEffect>();
        foreach (JObject effectData in equipmentData["effects"] as JArray ?? new JArray())
        {
            effects.Add(EquipmentEffect.FromJson(effectData));
        }
        this.effects = effects;
    }

    public void ApplyEffects(Player player)
    {
        foreach (EquipmentEffect effect in effects)
        {
            effect.ApplyEffect(player);
        }
    }
}