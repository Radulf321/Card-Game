using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

#nullable enable
public class FlagDictionary
{
    private Dictionary<string, bool> boolFlags;
    private Dictionary<string, int> intFlags;
    private Dictionary<string, float> floatFlags;
    private Dictionary<string, string> stringFlags;

    public FlagDictionary()
    {
        boolFlags = new Dictionary<string, bool>();
        intFlags = new Dictionary<string, int>();
        floatFlags = new Dictionary<string, float>();
        stringFlags = new Dictionary<string, string>();
    }

    public FlagDictionary(JObject json)
    {
        boolFlags = json["bool"]?.ToObject<Dictionary<string, bool>>() ?? new Dictionary<string, bool>();
        intFlags = json["int"]?.ToObject<Dictionary<string, int>>() ?? new Dictionary<string, int>();
        floatFlags = json["float"]?.ToObject<Dictionary<string, float>>() ?? new Dictionary<string, float>();
        stringFlags = json["string"]?.ToObject<Dictionary<string, string>>() ?? new Dictionary<string, string>();
    }

    public void Clear()
    {
        boolFlags.Clear();
        intFlags.Clear();
        floatFlags.Clear();
        stringFlags.Clear();
    }

    public void SetValue(string key, object value)
    {
        if (value is bool boolValue)
        {
            SetValue<bool>(key, boolValue);
        }
        else if (value is int intValue)
        {
            SetValue<int>(key, intValue);
        }
        else if (value is float floatValue)
        {
            SetValue<float>(key, floatValue);
        }
        else if (value is string stringValue)
        {
            SetValue<string>(key, stringValue);
        }
        else
        {
            throw new ArgumentException("Unsupported type for flag value.");
        }
    }

    public void SetValue<T>(string key, T value)
    {
        if (value is bool boolValue)
        {
            boolFlags[key] = boolValue;
        }
        else if (value is int intValue)
        {
            intFlags[key] = intValue;
        }
        else if (value is float floatValue)
        {
            floatFlags[key] = floatValue;
        }
        else if (value is string stringValue)
        {
            stringFlags[key] = stringValue;
        }
        else
        {
            throw new ArgumentException("Unsupported type for flag value.");
        }
    }

    public object? GetValue(string key)
    {
        if (boolFlags.ContainsKey(key))
        {
            return GetValue<bool>(key);
        }
        else if (intFlags.ContainsKey(key))
        {
            return GetValue<int>(key);
        }
        else if (floatFlags.ContainsKey(key))
        {
            return GetValue<float>(key);
        }
        else if (stringFlags.ContainsKey(key))
        {
            return GetValue<string>(key);
        }
        else
        {
            return null;
        }
    }

    public T? GetValue<T>(string key)
    {
        if (typeof(T) == typeof(bool))
        {
            if (!boolFlags.ContainsKey(key))
            {
                return default(T);
            }
            return (T)(object)boolFlags[key];
        }
        else if (typeof(T) == typeof(int))
        {
            if (!intFlags.ContainsKey(key))
            {
                return default(T);
            }
            return (T)(object)intFlags[key];
        }
        else if (typeof(T) == typeof(float))
        {
            if (!floatFlags.ContainsKey(key))
            {
                return default(T);
            }
            return (T)(object)floatFlags[key];
        }
        else if (typeof(T) == typeof(string))
        {
            if (!stringFlags.ContainsKey(key))
            {
                return default(T);
            }
            return (T)(object)stringFlags[key];
        }
        else
        {
            throw new KeyNotFoundException("Invalid type for GetValue: " + typeof(T).Name);
        }
    }

    public string ToJson()
    {
        return JObject.FromObject(new Dictionary<string, object>
        {
            {"bool", boolFlags},
            {"int", intFlags},
            {"float", floatFlags},
            {"string", stringFlags}            
        }).ToString();
    }
}