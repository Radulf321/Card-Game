using System.Collections.Generic;

#nullable enable
public class Turn
{
    private List<Requirement> requirements;
    private List<CardEffect> effects;

    public Turn(List<Requirement>? requirements = null, List<CardEffect>? effects = null)
    {
        this.requirements = requirements ?? new List<Requirement>();
        this.effects = effects ?? new List<CardEffect>();
    }

    public bool areRequirementsFulfilled()
    {
        foreach (Requirement requirement in requirements)
        {
            if (!requirement.isFulfilled())
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

    public List<CardEffect> getEffects()
    {
        return this.effects;
    }
}
