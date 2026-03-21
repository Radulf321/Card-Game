using Newtonsoft.Json.Linq;

#nullable enable
public abstract class RequirementFactory
{
    public static RequirementFactory FromJson(JObject json, CombatTarget? owner = null, GoalManager? goalManager = null)
    {
        string type = json["type"]?.ToString() ?? "Undefined";
        switch (type)
        {
            case "goal":
                return new GoalRequirementFactory(json, owner, goalManager);
            case "total":
                return new TotalRequirementFactory(json, owner, goalManager);
            default:
                throw new System.Exception("Requirement type not recognized: " + type);
        }
    }

    protected CombatTarget? owner;
    protected GoalManager? goalManager;

    private int? index;
    private int roundMin;
    private int roundMax;

    public RequirementFactory(JObject json, CombatTarget? owner = null, GoalManager? goalManager = null)
    {
        this.index = json["index"]?.ToObject<int>();
        this.roundMin = json["round"]?["min"]?.ToObject<int>() ?? int.MinValue;
        this.roundMax = json["round"]?["max"]?.ToObject<int>() ?? int.MaxValue;
        this.owner = owner;
        this.goalManager = goalManager;
    }

    public bool IsValid(int index, int round)
    {
        return ((this.index == null) || (this.index == index)) && (round >= roundMin) && (round <= roundMax);
    }

    public abstract Requirement CreateRequirement(int turn);
}