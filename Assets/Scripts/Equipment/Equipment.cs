using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

public class Equipment : IUnlockable
{
    private string id;
    private string name;
    private string iconPath;
    private string slot;
    private List<EquipmentEffect> effects = new List<EquipmentEffect>();
    private List<AvailableRequirement> requirements;

    public Equipment(string id, string name, string iconPath, List<EquipmentEffect> effects, string slot, List<AvailableRequirement> requirements)
    {
        this.slot = slot;
        this.id = id;
        this.name = name;
        this.iconPath = iconPath;
        this.effects = effects;
        this.requirements = requirements;
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
        List<AvailableRequirement> requirements = new List<AvailableRequirement>();
        if (equipmentData["requirements"] is JArray requirementsArray)
        {
            foreach (JObject requirementData in requirementsArray)
            {
                requirements.Add(AvailableRequirement.FromJson(requirementData, RequirementOrigin.Equipment));
            }
        }
        this.requirements = requirements;
    }

    public void ApplyEffects(Player player)
    {
        foreach (EquipmentEffect effect in effects)
        {
            effect.ApplyEffect(player);
        }
    }

    public string GetID()
    {
        return id;
    }

    public string GetSlot()
    {
        return slot;
    }

    public string GetIconPath()
    {
        return Game.Instance.GetResourcePath() + "/Graphics/Equipment/" + this.iconPath;
    }

    public List<Card> GetCards()
    {
        List<Card> cards = new List<Card>();
        foreach (EquipmentEffect effect in effects)
        {
            if (effect is CardsEquipmentEffect cardEffect)
            {
                cards.AddRange(cardEffect.GetCards());
            }
        }
        return cards;
    }

    public string GetName()
    {
        return this.name;
    }

    public async Task<string> GetOtherEffectText()
    {
        List<string> effectTexts = new List<string>();
        foreach (EquipmentEffect effect in effects)
        {
            if (!(effect is CardsEquipmentEffect))
            {
                effectTexts.Add(await effect.GetCaption());
            }
        }
        return string.Join(", ", effectTexts);
    }

    public bool IsInitialEquipment()
    {
        return this.requirements.Count == 0;
    }

    public bool IsAvailable()
    {
        foreach (AvailableRequirement requirement in requirements)
        {
            if (!requirement.IsAvailable(this))
            {
                return false;
            }
        }

        return true;
    }
}