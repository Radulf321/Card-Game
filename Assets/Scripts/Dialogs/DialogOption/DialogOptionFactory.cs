#nullable enable

using System.Collections.Generic;
using Newtonsoft.Json.Linq;

abstract public class DialogOptionFactory
{
    public static DialogOptionFactory FromJSON(JObject json)
    {
        switch (json["type"]?.ToString())
        {
            case "constant":
                return new ConstantDialogOptionFactory(json);
            // If it doesn't have a type, assume it's directly a dialog option
            default:
                return new ConstantDialogOptionFactory(new List<DialogOption>()
                {
                    new DialogOption(json)
                });
        }
    }

    abstract public List<DialogOption> GetOptions(bool allOptions = false);
}