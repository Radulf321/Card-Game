using System;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

#nullable enable

public enum RequirementType
{
    Goal, Total,
}

abstract public class Requirement : IClonable<Requirement>
{
    static public Requirement FromJson(JObject json, GoalManager? goalManager = null)
    {
        RequirementType? type = EnumHelper.ParseEnum<RequirementType>(json["type"]?.ToString());
        switch (type)
        {
            case RequirementType.Goal:
                return new GoalRequirement(json, goalManager);
            case RequirementType.Total:
                return new TotalRequirement(json, goalManager);
            default:
                throw new Exception("Unknown RequirementType");
        }
    }

    protected GoalManager? goalManager;

    protected Requirement(GoalManager? goalManager = null)
    {
        this.goalManager = goalManager;
    }

    public void SetGoalManager(GoalManager? goalManager)
    {
        this.goalManager = goalManager;
    }

    abstract public bool IsFulfilled();
    abstract public Task<string> GetDescription();
    abstract public Requirement Clone();

    protected GoalManager GetGoalManager()
    {
        GoalManager? goalManager = this.goalManager ?? CombatHandler.instance?.GetGoalManager();
        if (goalManager == null)
        {
            throw new Exception("No GoalManager available to check Requirement");
        }
        return goalManager;
    }

    public abstract JObject ToJson();
}
