
using System.Collections.Generic;
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
            T? parsedValue = property.Value.ToObject<T>();
            if (parsedValue != null)
            {
                result.Add(property.Name, parsedValue);
            }
        }
        return result;
    }
}