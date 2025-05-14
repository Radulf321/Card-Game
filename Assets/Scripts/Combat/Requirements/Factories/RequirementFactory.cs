using Newtonsoft.Json.Linq;

public abstract class RequirementFactory
{
    public static RequirementFactory FromJson(JObject json, CombatTarget owner)
    {
        string type = json["type"]?.ToString() ?? "Undefined";
        switch (type)
        {
            case "goal":
                return new GoalRequirementFactory(json, owner);
            case "total":
                return new TotalRequirementFactory(json, owner);
            default:
                throw new System.Exception("Requirement type not recognized: " + type);
        }
    }
    public abstract Requirement CreateRequirement(int turn);
}