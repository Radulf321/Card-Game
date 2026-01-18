using Newtonsoft.Json.Linq;

#nullable enable
public abstract class RequirementFactory
{
    public static RequirementFactory FromJson(JObject json, CombatTarget? owner = null, Enemy? enemy = null)
    {
        string type = json["type"]?.ToString() ?? "Undefined";
        switch (type)
        {
            case "goal":
                return new GoalRequirementFactory(json, owner, enemy);
            case "total":
                return new TotalRequirementFactory(json, owner, enemy);
            default:
                throw new System.Exception("Requirement type not recognized: " + type);
        }
    }

    protected CombatTarget? owner;
    protected Enemy? enemy;

    private int? index;
    private int roundMin;
    private int roundMax;

    public RequirementFactory(JObject json, CombatTarget? owner = null, Enemy? enemy = null)
    {
        this.index = json["index"]?.ToObject<int>();
        this.roundMin = json["round"]?["min"]?.ToObject<int>() ?? int.MinValue;
        this.roundMax = json["round"]?["max"]?.ToObject<int>() ?? int.MaxValue;
        this.owner = owner;
        this.enemy = enemy;
    }

    public bool IsValid(int index, int round)
    {
        return ((this.index == null) || (this.index == index)) && (round >= roundMin) && (round <= roundMax);
    }

    public abstract Requirement CreateRequirement(int turn);
}