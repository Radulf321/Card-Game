using System.Threading.Tasks;
using UnityEngine;
using Newtonsoft.Json.Linq;

public class ExperienceReward : Reward
{
    private AmountCalculation amount;
    private string experienceType;

    public ExperienceReward(AmountCalculation amount, string experienceType)
    {
        this.amount = amount;
        this.experienceType = experienceType;
    }

    public ExperienceReward(int amount, string experienceType) : this(
        amount: new ConstantAmountCalculation(amount),
        experienceType: experienceType)
    {
    }

    public ExperienceReward(JObject rewardData) : this(
        amount: AmountCalculation.FromJson(rewardData["amount"]),
        experienceType: rewardData["experienceType"]!.ToString()
    )
    {
    }

    public int GetAmount()
    {
        return this.amount.GetValue();
    }

    public override void Collect()
    {
        Game.Instance.GetCurrentCombatTarget().IncreaseExperience(experienceType, this.GetAmount());
    }

    public override Task<string> ToNiceString()
    {
        return Task.FromResult(
            Game.Instance.GetExperienceTypeName(this.experienceType)
        );
    }

    public override Task<string> GetCaption()
    {
        return Task.FromResult(
            "+" + this.GetAmount() + " " + Game.Instance.GetExperienceTypeName(this.experienceType)
        );
    }

    public override Sprite GetSprite()
    {
        return Game.Instance.GetExperienceTypeIcon(this.experienceType);
    }
}