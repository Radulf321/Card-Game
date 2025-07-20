using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine.Localization.Settings;

public class TotalRequirement : Requirement
{
    private int amount;

    public TotalRequirement(int amount) {
        this.amount = amount;
    }

    public override bool IsFulfilled() {
        return CombatHandler.instance.getTotal() >= amount;
    }

    public override Task<string> GetDescription()
    {
        return AsyncHelper.HandleToTask(
            LocalizationSettings.StringDatabase.GetLocalizedStringAsync("RequirementStrings", "TotalDescription",
            arguments: new Dictionary<string, object> {
                { "current", CombatHandler.instance.getTotal() },
                { "amount", this.amount },
            }
        ));
    }
}
