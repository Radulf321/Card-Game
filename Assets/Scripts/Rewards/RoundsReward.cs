using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.Localization.Settings;

public class RoundsReward : Reward
{
    private int amount;

    public RoundsReward(int amount = 1)
    {
        this.amount = amount;
    }

    public RoundsReward(JObject rewardData) : this(
        amount: rewardData["amount"]?.ToObject<int>() ?? 1
    )
    {
    }

    public int GetAmount()
    {
        return this.amount;
    }

    public override void Collect()
    {
        Game.Instance.AddRemainingRounds(GetAmount());
    }

    public override string ToNiceString()
    {
        return LocalizationSettings.StringDatabase.GetLocalizedString("RewardStrings", "RoundsShort");
    }

    public override string GetCaption()
    {
        return LocalizationSettings.StringDatabase.GetLocalizedString("RewardStrings", "RoundsCaption",
            arguments: new Dictionary<string, object> {
                { "amount", GetAmount() }
            }
        );
    }

    public override Sprite GetSprite()
    {
        return Game.Instance.GetIcon("Rounds");
    }
}