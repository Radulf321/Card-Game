#nullable enable

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;
using UnityEngine;

abstract public class DialogOptionFactory
{
    public static List<DialogOptionFactory> FromJson(JToken json)
    {
        List<DialogOptionFactory> result = new List<DialogOptionFactory>();
        switch (json.Type)
        {
            case JTokenType.Object:
                JObject jsonObject = (json as JObject)!;
                switch (json["type"]?.ToString())
                {
                    case "file":
                        result.AddRange(DialogOptionFactory.FromFile(jsonObject));
                        break;

                    case "constant":
                        result.Add(new ConstantDialogOptionFactory(jsonObject));
                        break;

                    case "random":
                        result.Add(new RandomDialogOptionFactory(jsonObject));
                        break;

                    case "conditional":
                        result.Add(new ConditionalDialogOptionFactory(jsonObject));
                        break;

                    // If it doesn't have a type, assume it's directly a dialog option
                    default:
                        result.Add(new ConstantDialogOptionFactory(new List<DialogOption>()
                            {
                                new DialogOption(jsonObject)
                            }));
                        break;
                }
                break;

            case JTokenType.Array:
                foreach (JToken child in json)
                {
                    result.AddRange(DialogOptionFactory.FromJson(child));
                }
                break;

            default:
                throw new System.Exception("Unexpected JSON for DialogOptionFactory: " + json.ToString());

        }
        return result;
    }

    public static List<DialogOptionFactory> FromFile(JObject json)
    {
        JToken optionData = JToken.Parse(Resources.Load<TextAsset>(Game.Instance.GetResourcePath() + "/Dialogs/" + json["file"]!.ToString()).text);
        return DialogOptionFactory.FromJson(JSONHelper.ReplaceVariables(optionData, JSONHelper.ObjectToDictionary<dynamic>(json)));
    }

    protected List<DialogOption> JoinFactories(List<DialogOptionFactory> factories, bool allOptions = false)
    {
        List<DialogOption> result = new List<DialogOption>();
        foreach (DialogOptionFactory factory in factories)
        {
            result.AddRange(factory.GetOptions(allOptions));
        }
        return result;
    }

    abstract public List<DialogOption> GetOptions(bool allOptions = false);
}