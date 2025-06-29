using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;

public class NotStatusCondition : StatusCondition
{
    private StatusCondition condition;

    public NotStatusCondition(JObject json)
    {
        this.condition = StatusCondition.FromJson(json["condition"] as JObject);
    }

    public override bool IsFulfilled()
    {
        return !condition.IsFulfilled();
    }
}