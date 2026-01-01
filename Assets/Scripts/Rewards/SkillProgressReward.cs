using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.Localization.Settings;

#nullable enable

public class SkillProgressReward : Reward
{
    private int amount;
    private string? skill;

    public SkillProgressReward(string? skill, int amount)
    {
        this.skill = skill;
        this.amount = amount;
    }

    public SkillProgressReward(JObject rewardData)
    {
        this.skill = rewardData["skill"]?.ToString();
        this.amount = rewardData["amount"]?.ToObject<int>() ?? 1;
    }

    public override Task<string> ToNiceString()
    {
        return AsyncHelper.HandleToTask(LocalizationSettings.StringDatabase.GetLocalizedStringAsync("RewardStrings", "SkillProgressShort"));
    }

    public override Task<string> GetCaption()
    {
        return AsyncHelper.HandleToTask(LocalizationSettings.StringDatabase.GetLocalizedStringAsync("RewardStrings", "SkillProgressCaption",
            arguments: new Dictionary<string, object> {
                { "amount", this.amount },
                { "numberSkills", Game.Instance.GetPlayer().GetSkills().Count }
            }
        ));
    }

    public override Sprite GetSprite()
    {
        return Game.Instance.GetIcon("SkillProgress")!;
    }

    public override void Collect()
    {
        foreach (Skill skill in Game.Instance.GetPlayer().GetSkills())
        {
            if (this.skill == null || skill.GetID() == this.skill)
            {
                skill.AddProgress(this.amount);
            }
        }
    }
}