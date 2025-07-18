using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
            case "currency":
                return new CurrencyReward(json);
            case "experience":
                return new ExperienceReward(json);
            case "buff":
                return new BuffReward(json);
            case "unlock":
                return new UnlockReward(json);
            default:
                throw new System.Exception("Reward type not recognized: " + type);
        }
    }
    abstract public void Collect();
    abstract public Task<string> ToNiceString();

    virtual public Task<string> GetCaption()
    {
        throw new NotImplementedException();
    }

    virtual public Sprite GetSprite()
    {
        throw new NotImplementedException();
    }
}