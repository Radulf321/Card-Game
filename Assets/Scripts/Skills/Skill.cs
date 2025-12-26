using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

public class Skill
{
    private List<GameEffect> effects;
    private string name;
    private string imagePath;
    private string id;
    private SkillCharge charge;

    public Skill(List<GameEffect> effects, string name, string imagePath, string id, SkillCharge charge)
    {
        this.effects = effects;
        this.name = name;
        this.imagePath = imagePath;
        this.id = id;
        this.charge = charge;
    }

    public Skill(JObject json)
    {
        List<GameEffect> effects = new List<GameEffect>();
        foreach (JObject effectJson in json["effects"]!.ToObject<List<JObject>>()!)
        {
            effects.Add(GameEffect.FromJson(effectJson));
        }
        this.effects = effects;
        this.name = LocalizationHelper.GetLocalizedString(json["name"] as JObject) ?? "No name";
        this.imagePath = json["image"]?.ToString() ?? "Placeholder";
        this.id = json["id"]!.ToString();
        this.charge = SkillCharge.FromJson(json["charge"]!.ToObject<JObject>()!, this);
    }

    public void Initialize()
    {
        this.charge.Initialize();
    }

    public bool CanUse()
    {
        return this.charge.GetCharges() > 0;
    }

    public void Use()
    {
        this.charge.UseCharge();
        foreach (GameEffect effect in this.effects)
        {
            effect.applyEffect();
        }
    }

    public string GetName()
    {
        return this.name;
    }

    public int GetCharges()
    {
        return this.charge.GetCharges();
    }

    public async Task<string> GetTextDescription()
    {
        List<string> descriptions = new List<string>();
        foreach (GameEffect effect in this.effects)
        {
            descriptions.Add(await effect.getDescription());
        }
        return string.Join("\n", descriptions);
    }

    public string GetImagePath(bool disabled)
    {
        return Game.Instance.GetResourcePath() + "/Graphics/Skills/" + this.imagePath + (disabled ? "Disabled" : "") ;
    }

    public string GetID()
    {
        return this.id;
    }

    public int GetProgress()
    {
        return this.charge.GetProgress();
    }

    public float GetProgressPercentual()
    {
        return this.charge.GetProgressPercentual();
    }

    public void AddProgress(int amount)
    {
        this.charge.AddProgress(amount);
    }
}