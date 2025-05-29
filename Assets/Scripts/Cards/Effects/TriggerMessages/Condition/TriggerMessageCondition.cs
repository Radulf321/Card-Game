using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

public abstract class TriggerMessageCondition
{
    static public TriggerMessageCondition FromJson(JObject json)
    {
        switch (json["type"].ToString())
        {
            case "addgoal":
                return new TriggerMessageConditionAddGoal(json);
            default:
                throw new System.Exception("Invalid condition type: " + json["type"].ToString());
        }
    }

    public abstract bool FulfillsCondition(TriggerMessage message);
    public abstract Task<string> GetDescription();
}