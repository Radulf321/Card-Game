using System;
using System.Collections.Generic;

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

    public void Clear()
    {
        boolFlags.Clear();
        intFlags.Clear();
        floatFlags.Clear();
        stringFlags.Clear();
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

    public T GetValue<T>(string key)
    {
        if (typeof(T) == typeof(bool))
        {
            return (T)(object)boolFlags[key];
        }
        else if (typeof(T) == typeof(int))
        {
            return (T)(object)intFlags[key];
        }
        else if (typeof(T) == typeof(float))
        {
            return (T)(object)floatFlags[key];
        }
        else if (typeof(T) == typeof(string))
        {
            return (T)(object)stringFlags[key];
        }
        else
        {
            throw new KeyNotFoundException("Invalid type for GetValue: " + typeof(T).Name);
        }
    }
}