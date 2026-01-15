using System.Collections.Generic;
using Newtonsoft.Json.Linq;

#nullable enable
public class Enemy
{
    private List<Requirement> requirements;
    private List<GameEffect> effects;
    private string imagePath;

    public Enemy(List<Requirement>? requirements = null, List<GameEffect>? effects = null, string imagePath = "")
    {
        this.requirements = requirements ?? new List<Requirement>();
        this.effects = effects ?? new List<GameEffect>();
        this.imagePath = imagePath;
    }

    public Enemy(JObject enemyData)
    {
        List<GameEffect> effects = new List<GameEffect>();
        foreach (JToken effect in (enemyData["effects"] as JArray) ?? new JArray())
        {
            effects.Add(GameEffect.FromJson(effect as JObject ?? new JObject()));
        }
        this.effects = effects;

        List<Requirement> requirements = new List<Requirement>();
        foreach (JToken requirement in (enemyData["requirements"] as JArray) ?? new JArray())
        {
            requirements.Add(RequirementFactory.FromJson(requirement as JObject ?? new JObject()).CreateRequirement(0));
        }
        this.requirements = requirements;
        this.imagePath = enemyData["image"]?.ToString() ?? "";
    }

    public bool AreRequirementsFulfilled()
    {
        foreach (Requirement requirement in requirements)
        {
            if (!requirement.IsFulfilled())
            {
                return false;
            }
        }
        return true;
    }

    public List<Requirement> GetRequirements()
    {
        return requirements;
    }

    public List<GameEffect> GetEffects()
    {
        return this.effects;
    }

    public string GetImagePath()
    {
        return Game.Instance.GetResourcePath() + "/Graphics/Enemies/" + this.imagePath;
    }
}
