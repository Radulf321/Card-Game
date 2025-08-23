using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using UnityEngine.Localization.Settings;

public abstract class TriggerMessageCondition
{
    static public TriggerMessageCondition FromJson(JObject json)
    {
        switch (json["type"].ToString().ToLower())
        {
            case "addgoal":
                return new TriggerMessageConditionAddGoal(json);
            case "startturn":
                return new TriggerMessageConditionStartTurn(json);
            case "endturn":
                return new TriggerMessageConditionEndTurn(json);
            case "talenttree":
            case "openscene":
                return new TriggerMessageConditionOpenScene(json);
            case "drawcards":
                return new TriggerMessageConditionDrawCards(json);
            default:
                throw new System.Exception("Invalid condition type: " + json["type"].ToString());
        }
    }

    public abstract bool FulfillsCondition(TriggerMessage message);
    public abstract Task<string> GetDescription();
    public virtual async Task<string> GetFullDescription(LimitType? limitType = null, int? limit = null)
    {
        string conditionDescription = await this.GetDescription();
        return await AsyncHelper.HandleToTask(LocalizationSettings.StringDatabase.GetLocalizedStringAsync("CardStrings", "CommonTrigger",
            arguments: new Dictionary<string, object> {
                { "condition", conditionDescription },
                { "limitType", limitType },
                { "limit", limit }
            })
        );
    }
}