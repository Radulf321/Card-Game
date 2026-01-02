using Newtonsoft.Json.Linq;

public abstract class StatusCondition
{
    static public StatusCondition FromJson(JObject json)
    {
        switch (json["type"].ToString())
        {
            case "turn":
                return new TurnStatusCondition(json);
            case "flag":
                return new FlagStatusCondition(json);
            case "cardInDeck":
                return new CardInDeckStatusCondition(json);
            case "not":
                return new NotStatusCondition(json);
            case "requirementType":
                return new RequirementTypeCondition(json);
            case "taskCompleted":
                return new TaskCompletedCondition(json);
            default:
                throw new System.Exception("Invalid condition type: " + json["type"].ToString());
        }
    }

    public abstract bool IsFulfilled();
}