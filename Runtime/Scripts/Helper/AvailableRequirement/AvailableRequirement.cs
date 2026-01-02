using System;
using Newtonsoft.Json.Linq;

public enum RequirementOrigin
{
    Equipment,
    CombatTarget,
    Location,
}

public abstract class AvailableRequirement
{
    public abstract bool IsAvailable(IUnlockable parent);

    public static AvailableRequirement FromJson(JObject json, RequirementOrigin? origin = null)
    {
        string type = json["type"]!.ToString();
        switch (type)
        {
            case "unlock":
                return new UnlockAvailableRequirement(json, origin.Value);
            default:
                throw new ArgumentException($"Unknown requirement type: {type}");
        }
    }
}