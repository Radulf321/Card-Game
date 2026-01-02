using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using UnityEngine.Localization.Settings;

#nullable enable
public class TalentTask : GameTask
{
    private string combatTargetID;
    private string talentID;

    public TalentTask(string id, List<Reward> rewards, string combatTargetID, string talentID) : base(id, rewards)
    {
        this.combatTargetID = combatTargetID;
        this.talentID = talentID;
    }

    public TalentTask(JObject json) : base(json)
    {
        this.combatTargetID = json["combatTarget"]!.ToString();
        this.talentID = json["talent"]!.ToString();
    }

    public override Task<string> GetDescription()
    {
        string combatTargetName = Game.Instance.GetCharacterManager().GetCombatTarget(this.combatTargetID)?.GetName() ?? "Unknown Target";
        string talentName = GetTalent()?.GetTitle() ?? "Unknown Talent";
        return AsyncHelper.HandleToTask(LocalizationSettings.StringDatabase.GetLocalizedStringAsync("TaskStrings", "TalentDescription",
            arguments: new Dictionary<string, object> {
                { "talent", talentName },
                { "combatTarget", combatTargetName }
            }
        ));
    }

    public override bool IsCompleted()
    {
        return GetTalent()?.IsPurchased() ?? false;
    }

    private Talent? GetTalent()
    {
        return Game.Instance.GetCharacterManager().GetCombatTarget(this.combatTargetID)?.GetTalent(this.talentID);
    }
}