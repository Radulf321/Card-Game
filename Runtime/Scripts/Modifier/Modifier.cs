using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using UnityEngine;

#nullable enable

public enum ModifierType
{
    Energy
}

public abstract class Modifier
{
    public static Modifier? FromJson(JObject? json)
    {
        if (json == null) {
            return null;  
        }
        ModifierType? type = EnumHelper.ParseEnum<ModifierType>(json["type"]?.ToString());
        
        switch (type)
        {
            case ModifierType.Energy:
                return new EnergyModifier(json);
            default:
                throw new System.Exception($"Unknown modifier type: {json["type"]}");
        }
    }

    public Modifier()
    {
    }

    public abstract int GetValue(int previousValue, ModifierType type, ContextInfo? context = null);

    public abstract Modifier Clone();

    public abstract Task<string> GetCaption();

    public abstract Sprite GetSprite();

    public abstract JObject ToJson();
}