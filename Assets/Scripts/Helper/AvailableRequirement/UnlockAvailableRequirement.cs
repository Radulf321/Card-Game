using System;
using Newtonsoft.Json.Linq;

public class UnlockAvailableRequirement : AvailableRequirement
{
    private RequirementOrigin origin;
    public UnlockAvailableRequirement(JObject json, RequirementOrigin origin)
    {
        this.origin = origin;
    }

    public override bool IsAvailable(IUnlockable parent)
    {
        switch (origin)
        {
            case RequirementOrigin.Equipment:
                return Game.Instance.GetEquipmentManager().IsEquipmentUnlocked(parent.GetID());;
            case RequirementOrigin.CombatTarget:
                throw new NotImplementedException();
            case RequirementOrigin.Location:
                throw new NotImplementedException();
            default:
                throw new ArgumentException($"Unknown requirement origin: {origin}");
        }
        throw new NotImplementedException();
    }
}