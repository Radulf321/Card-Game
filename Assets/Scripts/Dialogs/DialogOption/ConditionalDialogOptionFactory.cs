#nullable enable

using System.Collections.Generic;
using Newtonsoft.Json.Linq;

public class ConditionalDialogOptionFactory : DialogOptionFactory
{
    private List<DialogOptionFactory> options;
    private StatusCondition condition;

    public ConditionalDialogOptionFactory(List<DialogOptionFactory> options, StatusCondition condition)
    {
        this.options = options;
        this.condition = condition;
    }

    public ConditionalDialogOptionFactory(JObject json) : this(options: DialogOptionFactory.FromJson(json["options"]!), condition: StatusCondition.FromJson(json["condition"] as JObject))
    {
    }

    override public List<DialogOption> GetOptions(bool allOptions = false)
    {
        if (allOptions || condition.IsFulfilled())
        {
            return JoinFactories(options, allOptions);
        }
        else
        {
            return new List<DialogOption>();
        }
    }
}