
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

public interface IUnlockable
{
    abstract public string GetID();
    abstract public List<AvailableRequirement> GetRequirements();
}

public static class UnlockableExtensions
{
    public static List<AvailableRequirement> GetRequirementsFromJson(JObject json, RequirementOrigin origin)
    {
        List<AvailableRequirement> requirements = new List<AvailableRequirement>();
        if (json["requirements"] is JArray requirementsArray)
        {
            foreach (JObject requirementData in requirementsArray)
            {
                requirements.Add(AvailableRequirement.FromJson(requirementData, origin));
            }
        }
        return requirements;
    }

    public static bool IsAvailable(this IUnlockable unlockable)
    {
        foreach (AvailableRequirement requirement in unlockable.GetRequirements())
        {
            if (!requirement.IsAvailable(unlockable))
            {
                return false;
            }
        }

        return true;
    }
}