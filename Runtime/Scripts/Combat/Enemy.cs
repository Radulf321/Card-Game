using System.Collections.Generic;
using Newtonsoft.Json.Linq;

#nullable enable
public class Enemy : IClonable<Enemy>, IEffectOwner
{
    private List<GameEffect> effects;
    private string imagePath;
    private GoalManager goalManager;
    private GameEffect? plannedEffect;

    public Enemy(List<GameEffect>? effects = null, string imagePath = "", GoalManager? goalManager = null)
    {
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
            effects.Add(GameEffect.FromJson(effect as JObject ?? new JObject(), owner: this));
        }
        this.effects = effects;

        List<Requirement> requirements = new List<Requirement>();
        foreach (JToken requirement in (enemyData["requirements"] as JArray) ?? new JArray())
        {
            requirements.Add(RequirementFactory.FromJson(requirement as JObject ?? new JObject(), goalManager: this.goalManager).CreateRequirement(0));
        }
        this.goalManager = new GoalManager(requirements);
        this.goalManager.SetOnGoalAdded(OnGoalAdded);
        this.imagePath = enemyData["image"]?.ToString() ?? "";
    }

    public List<Requirement> GetRequirements()
    {
        return this.goalManager.GetRequirements();
    }

    public void SetEffects(List<GameEffect> newEffects)
    {
        this.effects = newEffects;
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
        foreach (Requirement requirement in this.goalManager.GetRequirements())
        {
            clonedRequirements.Add(requirement.Clone());
        }

        Enemy clonedEnemy = new Enemy(imagePath: this.imagePath, goalManager: this.goalManager.Clone());
        
        List<GameEffect> clonedEffects = new List<GameEffect>();
        foreach (GameEffect effect in this.effects)
        {
            clonedEffects.Add(effect.Clone(clonedEnemy));
        }
        clonedEnemy.SetEffects(clonedEffects);
        return clonedEnemy;
    }

    public void PlanEffect()
    {
        if (this.effects.Count > 0)
        {
            this.plannedEffect = this.effects[UnityEngine.Random.Range(0, this.effects.Count)].Clone(null);
        }
    }

    public void ExecuteEffect()
    {
        this.plannedEffect?.applyEffect();
        this.plannedEffect = null;
    }

    public GameEffect? GetPlannedEffect()
    {
        return this.plannedEffect;
    }

    private void OnGoalAdded()
    {
        if (this.goalManager.AreRequirementsFulfilled())
        {
            CombatHandler.instance?.RemoveEnemy(this);
        }
    }
}