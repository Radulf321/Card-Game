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

    private int? index;
    private int roundMin;
    private int roundMax;

    public RequirementFactory(JObject json)
    {
        this.index = json["index"]?.ToObject<int>();
        this.roundMin = json["round"]?["min"]?.ToObject<int>() ?? int.MinValue;
        this.roundMax = json["round"]?["max"]?.ToObject<int>() ?? int.MaxValue;
    }

    public bool IsValid(int index, int round)
    {
        return ((this.index == null) || (this.index == index)) && (round >= roundMin) && (round <= roundMax);
    }

    public abstract Requirement CreateRequirement(int turn);
}