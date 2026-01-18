using System.Collections.Generic;
using Newtonsoft.Json.Linq;

#nullable enable
public class Enemy : IClonable<Enemy>
{
    private List<Requirement> requirements;
    private List<GameEffect> effects;
    private string imagePath;
    private GoalManager goalManager;

    public Enemy(List<Requirement>? requirements = null, List<GameEffect>? effects = null, string imagePath = "", GoalManager? goalManager = null)
    {
        this.requirements = requirements ?? new List<Requirement>();
        foreach (Requirement requirement in this.requirements)
        {
            requirement.SetEnemy(this);
        }
        this.effects = effects ?? new List<GameEffect>();
        this.imagePath = imagePath;
        this.goalManager = goalManager ?? new GoalManager();
        this.goalManager.SetOnGoalAdded(OnGoalAdded);
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
            requirements.Add(RequirementFactory.FromJson(requirement as JObject ?? new JObject(), enemy: this).CreateRequirement(0));
        }
        this.requirements = requirements;
        this.imagePath = enemyData["image"]?.ToString() ?? "";
        this.goalManager = new GoalManager();
        this.goalManager.SetOnGoalAdded(OnGoalAdded);
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

    public GoalManager GetGoalManager()
    {
        return this.goalManager;
    }

    public Enemy Clone()
    {
        List<Requirement> clonedRequirements = new List<Requirement>();
        foreach (Requirement requirement in this.requirements)
        {
            clonedRequirements.Add(requirement.Clone());
        }

        List<GameEffect> clonedEffects = new List<GameEffect>();
        foreach (GameEffect effect in this.effects)
        {
            clonedEffects.Add(effect.Clone(null));
        }

        return new Enemy(clonedRequirements, clonedEffects, this.imagePath, this.goalManager.Clone());
    }

    private void OnGoalAdded()
    {
        foreach (Requirement requirement in requirements)
        {
            if (!requirement.IsFulfilled())
            {
                return;
            }
        }
        CombatHandler.instance?.RemoveEnemy(this);
    }
}