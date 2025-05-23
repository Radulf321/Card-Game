using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.Localization.Settings;

public class EnergyReward : Reward
{
    private int amount;
    private int turn;

    public EnergyReward(int amount = 1, int turn = 0)
    {
        this.amount = amount;
        this.turn = turn;
    }

    public EnergyReward(JObject rewardData) : this(
        amount: rewardData["amount"]?.ToObject<int>() ?? 1,
        turn: rewardData["turn"]?.ToObject<int>() ?? 0
    )
    {
    }

    public int GetTurn()
    {
        return this.turn;
    }

    public int GetAmount()
    {
        return this.amount;
    }

    public override void Collect()
    {
        Game.Instance.GetPlayer().AddEnergy(GetAmount(), GetTurn());
    }

    public override string ToNiceString()
    {
        return LocalizationSettings.StringDatabase.GetLocalizedString("RewardStrings", "EnergyShort");
    }

    public override string GetCaption()
    {
        return LocalizationSettings.StringDatabase.GetLocalizedString("RewardStrings", "EnergyCaption",
            arguments: new Dictionary<string, object> {
                { "amount", GetAmount() },
                { "turn", GetTurn() }
            }
        );
    }

    public override Sprite GetSprite()
    {
        return Game.Instance.GetIcon("Energy");
    }
}