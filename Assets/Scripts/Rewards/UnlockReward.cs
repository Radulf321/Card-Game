using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using UnityEngine;

#nullable enable
public class UnlockReward : Reward
{
    private RequirementOrigin origin;
    private string unlockID;

    public UnlockReward(RequirementOrigin origin, string unlockID)
    {
        this.origin = origin;
        this.unlockID = unlockID;
    }

    public UnlockReward(JObject rewardData) : this(origin: EnumHelper.ParseEnum<RequirementOrigin>(rewardData["unlock"]!["type"]!.ToString())!.Value, unlockID: rewardData["unlock"]!["id"]!.ToString())
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
            case RequirementOrigin.Location:
                Game.Instance.GetCharacterManager().UnlockCharacter(this.unlockID);
                break;

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
                return Task.FromResult(Game.Instance.GetEquipmentManager().GetEquipment(this.unlockID)?.GetName() ?? "Unlocked Equipment does not exist");

            case RequirementOrigin.CombatTarget:
            case RequirementOrigin.Location:
                return Task.FromResult(Game.Instance.GetCharacterManager().GetActionCharacter(this.unlockID)?.GetName() ?? "Unlocked Character does not exist");

            default:
                throw new System.ArgumentException($"Unknown requirement origin: {this.origin}");
        }
    }

    public override Sprite GetSprite()
    {
        switch (this.origin)
        {
            case RequirementOrigin.Equipment:
                return Resources.Load<Sprite>(Game.Instance.GetEquipmentManager().GetEquipment(this.unlockID)!.GetIconPath());

            case RequirementOrigin.CombatTarget:
            case RequirementOrigin.Location:
                return Resources.Load<Sprite>(Game.Instance.GetCharacterManager().GetActionCharacter(this.unlockID)!.GetDialogOption()!.GetImagePath());

            default:
                throw new System.ArgumentException($"Unknown requirement origin: {this.origin}");
        }
    }
}