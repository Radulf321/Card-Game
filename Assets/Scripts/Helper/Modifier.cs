using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.Localization.Settings;

public enum ModifierType
{
    Energy
}

public class Modifier
{
    private ModifierType type;
    private int value;
    private int? turn;

    public Modifier(ModifierType type, int value = 1, int? turn = null)
    {
        this.type = type;
        this.value = value;
        this.turn = turn;
    }

    public Modifier(JObject json) : this(
        type: EnumHelper.ParseEnum<ModifierType>(json["type"].ToString()).Value,
        value: json["value"]?.ToObject<int>() ?? 1,
        turn: json["turn"]?.ToObject<int>()
    )
    {
    }

    public int GetValue(int previousValue, ModifierType type, int? turn = null)
    {
        if ((this.type == type) && (this.turn == turn))
        {
            return previousValue + this.value;
        }
        else
        {
            return previousValue;
        }
    }

    public Modifier Clone()
    {
        return new Modifier(this.type, this.value, this.turn);
    }

    public Task<string> GetCaption()
    {
        switch (this.type)
        {
            case ModifierType.Energy:
                return AsyncHelper.HandleToTask(LocalizationSettings.StringDatabase.GetLocalizedStringAsync("RewardStrings", "EnergyCaption", arguments: new Dictionary<string, object> {
                { "amount", this.value },
                { "turn", this.turn ?? 0 }
            }));

            default:
                throw new System.Exception("Unknown modifier type trying to get caption: " + this.type);

        }

    }

    public Sprite GetSprite()
    {
        {
            switch (this.type)
            {
                case ModifierType.Energy:
                    return Game.Instance.GetIcon("Energy");

                default:
                    throw new System.Exception("Unknown modifier type trying to get sprite: " + this.type);

            }
        }
    }
}