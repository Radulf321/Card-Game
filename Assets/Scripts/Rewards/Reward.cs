using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using UnityEngine;

public abstract class Reward
{
    public static List<Reward> FromJson(JArray json)
    {
        return json.Select(jsonElement => Reward.FromJson(jsonElement as JObject)).ToList();
    }

    public static Reward FromJson(JObject json)
    {
        string type = json["type"]?.ToString() ?? "Undefined";
        switch (type)
        {
            case "card":
                return new CardReward(json);
            case "energy":
                return new EnergyReward(json);
            case "rounds":
                return new RoundsReward(json);
            default:
                throw new System.Exception("Reward type not recognized: " + type);
        }
    }
    abstract public void Collect();
    abstract public string ToNiceString();

    virtual public string GetCaption()
    {
        throw new NotImplementedException();
    }

    virtual public Sprite GetSprite()
    {
        throw new NotImplementedException();
    }
}