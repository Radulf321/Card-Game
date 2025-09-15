using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using UnityEngine.Localization.Settings;

#nullable enable

public class RequirementGoalEffect : GameEffect
{
    private Dictionary<string, int> requirements;

    public RequirementGoalEffect(Dictionary<string, int> requirements)
    {
        this.requirements = requirements;
    }

    public RequirementGoalEffect(JObject json)
    {
        Dictionary<string, int> requirements = new Dictionary<string, int>();
        foreach (JProperty requirement in ((json["requirements"] as JObject) ?? new JObject()).Properties())
        {
            requirements.Add(requirement.Name, requirement.Value.ToObject<int>());
        }
        this.requirements = requirements;
    }

    public override void applyEffect()
    {
        // It's only a requirement to play, so no effects
    }

    public override bool canPlay()
    {
        CombatHandler? combatHandler = CombatHandler.instance;
        foreach (KeyValuePair<string, int> requirement in this.requirements)
        {
            if (combatHandler?.getGoalAmountThisTurn(requirement.Key) < requirement.Value)
            {
                return false;
            }
        }
        return true;
    }

    public override GameEffect Clone(Card? newOwner)
    {
        return new RequirementGoalEffect(this.requirements);
    }

    public override Task<string> getDescription()
    {
        List<string> requirementStrings = new List<string>();
        foreach (KeyValuePair<string, int> requirement in this.requirements)
        {
            requirementStrings.Add(requirement.Value.ToString() + " " + Game.Instance.GetGoalName(requirement.Key));
        }
        return AsyncHelper.HandleToTask(LocalizationSettings.StringDatabase.GetLocalizedStringAsync("CardStrings", "RequirementGoal",
            arguments: new Dictionary<string, object> {
                { "requirements", requirementStrings },
                { "length", requirementStrings.Count },
            }
        ));
    }

    public override Task<string?> GetInternalIconDescription()
    {
        List<string> requirementStrings = new List<string>();
        foreach (KeyValuePair<string, int> requirement in this.requirements)
        {
            requirementStrings.Add(requirement.Value.ToString() + Game.Instance.GetGoalInlineIcon(requirement.Key));
        }
        return AsyncHelper.HandleToTask(LocalizationSettings.StringDatabase.GetLocalizedStringAsync("CardStrings", "RequirementGoalIcon",
            arguments: new Dictionary<string, object> {
                { "requirements", requirementStrings },
            }
        ));
    }
}