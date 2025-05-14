using System;
using Newtonsoft.Json.Linq;

public class TotalRequirementFactory : RequirementFactory
{
    private CombatTarget owner;

    public TotalRequirementFactory(JObject json, CombatTarget owner) {
        this.owner = owner;
    }
    public override Requirement CreateRequirement(int turn) {
        double multiplier = ((turn / 2) + 1) * Math.Pow(1.2, this.owner.GetLevel()) * UnityEngine.Random.Range(0.9f, 1.1f);
        return new TotalRequirement((int)(5 * multiplier));
    }
}