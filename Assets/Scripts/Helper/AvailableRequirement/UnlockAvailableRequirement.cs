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
        UnityEngine.Debug.Log($"Unlockable available? {parent.GetID()} - {origin}");
        switch (origin)
        {
            case RequirementOrigin.Equipment:
                return Game.Instance.GetEquipmentManager().IsEquipmentUnlocked(parent.GetID());

            case RequirementOrigin.CombatTarget:
            case RequirementOrigin.Location:
                return Game.Instance.GetCharacterManager().IsCharacterUnlocked(parent.GetID());

            default:
                throw new ArgumentException($"Unknown requirement origin: {origin}");
        }
        throw new NotImplementedException();
    }
}