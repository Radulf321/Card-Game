#nullable enable

using System.Collections.Generic;
using Newtonsoft.Json.Linq;

public class ConstantDialogOptionFactory : DialogOptionFactory
{
    private List<DialogOption> options;

    public ConstantDialogOptionFactory(List<DialogOption> options)
    {
        this.options = options;
    }

    public ConstantDialogOptionFactory(JObject json)
    {
        List<DialogOption> options = new List<DialogOption>();
        foreach (JToken option in (json["options"] as JArray) ?? new JArray())
        {
            options.Add(new DialogOption((option as JObject)!));
        }
        this.options = options;
    }

    override public List<DialogOption> GetOptions(bool allOptions = false)
    {
        return options;
    }
}