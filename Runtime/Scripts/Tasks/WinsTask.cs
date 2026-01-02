using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using UnityEngine.Localization.Settings;

#nullable enable

public class WinsTask : GameTask
{
    private int amount;
    private string? combatTargetID;

    public WinsTask(string id, List<Reward> rewards, int amount, string? combatTargetID = null) : base(id, rewards)
    {
        this.amount = amount;
        this.combatTargetID = combatTargetID;
    }

    public WinsTask(JObject json) : base(json)
    {
        this.amount = json["amount"]!.ToObject<int>();
        this.combatTargetID = json["combatTarget"]?.ToString();
    }

    public override Task<string> GetDescription()
    {
        string? combatTargetName = Game.Instance.GetCharacterManager().GetCombatTarget(this.combatTargetID)?.GetName();
        return AsyncHelper.HandleToTask(LocalizationSettings.StringDatabase.GetLocalizedStringAsync("TaskStrings", "WinDescription",
            arguments: new Dictionary<string, object?> {
                { "amount", this.amount },
                { "combatTarget", combatTargetName }
            }
        ));
    }

    public override int? GetProgress()
    {
        int wins;
        if (this.combatTargetID == null)
        {
            wins = Game.Instance.GetTaskManager().GetTotalWins();
        }
        else
        {
            wins = Game.Instance.GetTaskManager().GetWins(combatTargetID);
        }

        return Math.Min(wins, this.amount);
        
    }

    public override int? GetTotal()
    {
        return this.amount;
    }
}