using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.Localization.Settings;

public class BuffReward : Reward
{
    private BuffFactory buffFactory;

    public BuffReward(BuffFactory buffFactory)
    {
        this.buffFactory = buffFactory;
    }

    public BuffReward(JObject rewardData) : this(buffFactory: new BuffFactory(rewardData["buff"] as JObject))
    {
    }

    public override void Collect()
    {
        // Buff registers and unregisters itself, so we don't need to save it anywhere
        buffFactory.CreateBuff();
    }

    public override Task<string> ToNiceString()
    {
        return AsyncHelper.HandleToTask(LocalizationSettings.StringDatabase.GetLocalizedStringAsync("RewardStrings", "BuffShort"));
    }

    public override Task<string> GetCaption()
    {
        return buffFactory.GetCaption();
    }

    public override Sprite GetSprite()
    {
        return buffFactory.GetSprite();
    }
}