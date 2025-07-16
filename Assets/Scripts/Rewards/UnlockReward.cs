using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.Localization.Settings;

public class UnlockReward : Reward
{
    private RequirementOrigin origin;
    private string unlockID;

    public UnlockReward(RequirementOrigin origin, string unlockID)
    {
        this.origin = origin;
        this.unlockID = unlockID;
    }

    public UnlockReward(JObject rewardData) : this(origin: EnumHelper.ParseEnum<RequirementOrigin>(rewardData["unlock"]["type"].ToString()).Value, unlockID: rewardData["unlock"]["id"].ToString())
    {
    }

    public override void Collect()
    {
        switch (this.origin)
        {
            case RequirementOrigin.Equipment:
                Game.Instance.GetEquipmentManager().UnlockEquipment(this.unlockID);
                break;

            case RequirementOrigin.CombatTarget:
                throw new System.NotImplementedException("Combat target unlocking is not implemented yet.");

            case RequirementOrigin.Location:
                throw new System.NotImplementedException("Location unlocking is not implemented yet.");

            default:
                throw new System.ArgumentException($"Unknown requirement origin: {this.origin}");
        }
    }

    public override Task<string> ToNiceString()
    {
        throw new System.NotImplementedException("UnlockReward does not support ToNiceString.");
    }

    public override Task<string> GetCaption()
    {
        switch (this.origin)
        {
            case RequirementOrigin.Equipment:
                return Task.FromResult(Game.Instance.GetEquipmentManager().GetEquipment(this.unlockID).GetName());

            case RequirementOrigin.CombatTarget:
                throw new System.NotImplementedException("Combat target unlocking is not implemented yet.");

            case RequirementOrigin.Location:
                throw new System.NotImplementedException("Location unlocking is not implemented yet.");

            default:
                throw new System.ArgumentException($"Unknown requirement origin: {this.origin}");
        }
    }

    public override Sprite GetSprite()
    {
        switch (this.origin)
        {
            case RequirementOrigin.Equipment:
                return Resources.Load<Sprite>(Game.Instance.GetEquipmentManager().GetEquipment(this.unlockID).GetIconPath());

            case RequirementOrigin.CombatTarget:
                throw new System.NotImplementedException("Combat target unlocking is not implemented yet.");

            case RequirementOrigin.Location:
                throw new System.NotImplementedException("Location unlocking is not implemented yet.");

            default:
                throw new System.ArgumentException($"Unknown requirement origin: {this.origin}");
        }
    }
}