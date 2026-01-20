
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;

#nullable enable
public static class JSONHelper
{
    public static Dictionary<string, T>? ObjectToDictionary<T>(JObject? jsonObject)
    {
        if (jsonObject == null)
        {
            return null;
        }
        Dictionary<string, T> result = new Dictionary<string, T>();
        foreach (JProperty property in jsonObject.Properties())
        {
            T? parsedValue;
            if (typeof(T) == typeof(AmountCalculation))
            {
                parsedValue = (T?)(object?)AmountCalculation.FromJson(property.Value);
            }
            else
            {
                parsedValue = property.Value.ToObject<T>();
            }
            if (parsedValue != null)
            {
                result.Add(property.Name, parsedValue);
            }
        }
        return result;
    }

    public static object ReplaceVariablesInString(string content, Dictionary<string, object>? values)
    {
        if (values == null)
        {
            return content;
        }
        Match match = Regex.Match(content, @"^\$\{(\w+)\}$");
        if (match.Success)
        {
            string variableName = match.Groups[1].Value;
            // Return the raw value from json, preserving its type
            return values[variableName] ?? content;
        }
        else
        {
            // Replace all ${variable} with the value from json
            content = Regex.Replace(content, @"\$\{(\w+)\}", match =>
            {
                string variableName = match.Groups[1].Value;
                return values[variableName]?.ToString() ?? match.Value;
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
    }

    public static JToken ReplaceVariables(JToken token, Dictionary<string, object>? values)
    {
        if (values == null)
        {
            return token;
        }
        switch (token.Type)
        {
            case JTokenType.String:
                return JToken.FromObject(ReplaceVariablesInString(token.ToString(), values));

            case JTokenType.Object:
                JObject newObject = new JObject();
                foreach (JProperty property in (token as JObject)?.Properties() ?? new List<JProperty>())
                {
                    newObject[property.Name] = ReplaceVariables(property.Value, values);
                }
                return newObject;

            case JTokenType.Array:
                JArray newArray = new JArray();
                foreach (JToken child in token)
                {
                    newArray.Add(ReplaceVariables(child, values));
                }
                return newArray;

            default:
                return token;
        }
    }
}