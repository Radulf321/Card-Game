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
        JToken ReplaceVariables(JToken token)
        {
            switch (token.Type)
            {
                case JTokenType.String:
                    string content = token.ToString();
                    Match match = Regex.Match(content, @"^\$\{(\w+)\}$");
                    if (match.Success)
                    {
                        string variableName = match.Groups[1].Value;
                        // Return the raw value from json, preserving its type
                        return json[variableName] ?? token;
                    }
                    else
                    {
                        // Replace all ${variable} with the value from json
                        content = Regex.Replace(content, @"\$\{(\w+)\}", match =>
                        {
                            string variableName = match.Groups[1].Value;
                            return json[variableName]?.ToString() ?? match.Value;
                        });

                        // Apply modificators like *{value, -1}
                        Match modificatorMatch = Regex.Match(content, @"^\*\{([0-9.,\-\s]+)\}$");
                        if (modificatorMatch.Success)
                        {
                            List<float> parts = new List<string>(modificatorMatch.Groups[1].Value.Split(',')).ConvertAll<float>(float.Parse);
                            float product = 1f;
                            foreach (float part in parts)
                            {
                                product *= part;
                            }
                            return product;
                        }
                        else
                        {
                            // Replace all ${variable} with the value from json
                            content = Regex.Replace(content, @"\*\{([0-9.,\-\s]+)\}", match =>
                            {
                                List<float> parts = new List<string>(modificatorMatch.Groups[1].Value.Split(',')).ConvertAll<float>(float.Parse);
                                float product = 1f;
                                foreach (float part in parts)
                                {
                                    product *= part;
                                }
                                return product.ToString();
                            });
                        }

                        return content;
                    }

                case JTokenType.Object:
                    JObject newObject = new JObject();
                    foreach (JProperty property in (token as JObject)?.Properties() ?? new List<JProperty>())
                    {
                        newObject[property.Name] = ReplaceVariables(property.Value);
                    }
                    return newObject;

                case JTokenType.Array:
                    JArray newArray = new JArray();
                    foreach (JToken child in token)
                    {
                        newArray.Add(ReplaceVariables(child));
                    }
                    return newArray;

                default:
                    return token;
            }
        }
        return DialogOptionFactory.FromJson(ReplaceVariables(optionData));
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