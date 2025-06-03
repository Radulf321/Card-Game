using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

public abstract class StatusCondition
{
    static public StatusCondition FromJson(JObject json)
    {
        switch (json["type"].ToString())
        {
            default:
                throw new System.Exception("Invalid condition type: " + json["type"].ToString());
        }
    }

    public abstract bool IsFulfilled();
}