using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.Localization.Settings;

#nullable enable

public class EnergyModifier : Modifier
{
    private int value;
    private int? turn;

    public EnergyModifier(int value = 1, int? turn = null)
    {
        this.value = value;
        this.turn = turn;
    }

    public EnergyModifier(JObject json) : this(
        value: json["value"]?.ToObject<int>() ?? 1,
        turn: json["turn"]?.ToObject<int>()
    )
    {
    }

    public override int GetValue(int previousValue, ModifierType type, ContextInfo? context = null)
    {
        if ((type == ModifierType.Energy) && (this.turn == context?.GetTurn()))
        {
            return previousValue + this.value;
        }
        else
        {
            return previousValue;
        }
    }

    public override Modifier Clone()
    {
        return new EnergyModifier(this.value, this.turn);
    }

    public override Task<string> GetCaption()
    {
        return AsyncHelper.HandleToTask(LocalizationSettings.StringDatabase.GetLocalizedStringAsync(
            "RewardStrings",
            "EnergyCaption",
            arguments: new Dictionary<string, object>
            {
                { "amount", this.value },
                { "turn", this.turn ?? 0 }
            }
        ));
    }

    public override Sprite GetSprite()
    {
        return Game.Instance.GetIcon("Energy");
    }

    public override JObject ToJson()
    {
        return new JObject
        {
            ["type"] = ModifierType.Energy.ToString(),
            ["value"] = this.value,
            ["turn"] = this.turn
        };
    }
}
