using System.Collections.Generic;

#nullable enable
public class Turn
{
    private List<Requirement> requirements;
    private List<GameEffect> effects;

    public Turn(List<Requirement>? requirements = null, List<GameEffect>? effects = null)
    {
        this.requirements = requirements ?? new List<Requirement>();
        this.effects = effects ?? new List<GameEffect>();
    }

    public bool areRequirementsFulfilled()
    {
        foreach (Requirement requirement in requirements)
        {
            if (!requirement.IsFulfilled())
            {
                return false;
            }
        }
        return true;
    }

    public List<Requirement> getRequirements()
    {
        return requirements;
    }

    public List<GameEffect> getEffects()
    {
        return this.effects;
    }
}
