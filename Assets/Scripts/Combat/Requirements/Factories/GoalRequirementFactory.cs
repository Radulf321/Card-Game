using System;
using Newtonsoft.Json.Linq;

public class GoalRequirementFactory : RequirementFactory
{
    private string goal;
    private CombatTarget owner;

    public GoalRequirementFactory(JObject json, CombatTarget owner) {
        this.goal = json["goal"]?.ToString() ?? "Undefined";
        this.owner = owner;
    }
    public override Requirement CreateRequirement(int turn) {
        double multiplier = ((turn / 2) + 1) * Math.Pow(1.2, this.owner.GetLevel()) * UnityEngine.Random.Range(0.9f, 1.1f);
        return new GoalRequirement(this.goal, (int)(3 * multiplier));
    }
}