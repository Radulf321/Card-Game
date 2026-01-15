using System.Collections.Generic;
using Newtonsoft.Json.Linq;

public class RequirementTypeCondition : StatusCondition
{
    private string requirementType;
    private RequirementComparison? comparison;

    public RequirementTypeCondition(JObject json)
    {
        this.requirementType = json["requirementType"].ToString();
        this.comparison = EnumHelper.ParseEnum<RequirementComparison>(json["comparison"].ToString());
    }

    public override bool IsFulfilled()
    {
        foreach (Turn turn in CombatHandler.instance.getTurns() ?? new List<Turn>())
        {
            foreach (Requirement requirement in turn.getRequirements()) {
                switch (requirementType)
                {
                    case "goal":
                        if ((requirement is GoalRequirement goalRequirement) && ((comparison == null) || (goalRequirement.GetComparison() == comparison)))
                        {
                            return true;
                        }
                        break;

                    case "total":
                        if (requirement is TotalRequirement)
                        {
                            return true;
                        }
                        break;
                }
            }
        }
        return false;
    }
}