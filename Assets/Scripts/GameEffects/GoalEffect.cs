using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using UnityEngine.Localization.Settings;

#nullable enable

public class GoalEffect : GameEffect
{

    private Card owner;
    private AmountCalculation amountCalculation;
    private string goal;
    private CardEffectTrigger trigger;

    public GoalEffect(string goal, AmountCalculation amountCalculation, Card owner, CardEffectTrigger trigger = CardEffectTrigger.PlayCard)
    {
        this.goal = goal;
        this.amountCalculation = amountCalculation;
        this.owner = owner;
        this.trigger = trigger;
    }

    public GoalEffect(string goal, int amount, Card owner, CardEffectTrigger trigger = CardEffectTrigger.PlayCard) : this(goal, new ConstantAmountCalculation(amount), owner, trigger)
    {
    }

    public GoalEffect(JObject json, Card owner, CardEffectTrigger trigger = CardEffectTrigger.PlayCard) : this(json["goal"]?.ToString() ?? "unknown", AmountCalculation.FromJson(json["amount"]) ?? new ConstantAmountCalculation(0), owner, trigger)
    {
    }

    public override void applyEffect()
    {
        // Assuming RoundHandler has a method to apply the effect
        CombatHandler.instance?.addGoal(goal, this.amountCalculation.GetValue(this.owner), this.trigger);
    }

    public override GameEffect Clone(Card? newOwner)
    {
        return new GoalEffect(this.goal, this.amountCalculation, newOwner ?? this.owner, this.trigger);
    }

    private async Task<string> GetDescription(string tableEntry, string? overridePrefix = null, string? overrideSuffix = null)
    {
        return await AsyncHelper.HandleToTask(LocalizationSettings.StringDatabase.GetLocalizedStringAsync("CardStrings", tableEntry,
            arguments: new Dictionary<string, object> {
                { "prefix", overridePrefix ?? (await this.amountCalculation.GetDescriptionPrefix(this.owner)) },
                { "goal", Game.Instance.GetGoalInlineIcon(this.goal) },
                { "suffix", overrideSuffix ?? (await this.amountCalculation.GetDescriptionSuffix(this.owner)) }
            }
        ));
    }
    
    public override async Task<string?> GetInternalIconDescription()
    {
        return await GetDescription(
            "GoalIcon",
            overridePrefix: await this.amountCalculation.GetDescriptionPrefixIcon(this.owner),
            overrideSuffix: await this.amountCalculation.GetDescriptionSuffixIcon(this.owner)
        );
    }

    public override Task<string> getDescription()
    {
        return GetDescription("Goal");
    }

    public override Task<string> getTriggerDescription()
    {
        return GetDescription("GoalTrigger");
    }
}