using System.Collections.Generic;

public class Turn
{
    private List<Requirement> requirements;

    public Turn() {
        requirements = new List<Requirement>();
    }

    public Turn(List<Requirement> requirements) {
        this.requirements = requirements;
    }

    public bool areRequirementsFulfilled() {
        foreach (Requirement requirement in requirements) {
            if (!requirement.isFulfilled()) {
                return false;
            }
        }
        return true;
    }

    public List<Requirement> getRequirements() {
        return requirements;
    }
}
