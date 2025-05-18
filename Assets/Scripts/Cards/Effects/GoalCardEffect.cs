using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine.Localization.Settings;

public class GoalCardEffect : CardEffect
{

    private Card owner;
    private AmountCalculation amountCalculation;
    private string goal;
    private CardEffectTrigger trigger;

    public GoalCardEffect(string goal, AmountCalculation amountCalculation, Card owner, CardEffectTrigger trigger = CardEffectTrigger.PlayCard)
    {
        this.goal = goal;
        this.amountCalculation = amountCalculation;
        this.owner = owner;
    }

    public GoalCardEffect(string goal, int amount, Card owner, CardEffectTrigger trigger = CardEffectTrigger.PlayCard) : this(goal, new ConstantAmountCalculation(amount), owner, trigger)
    {
    }

    public GoalCardEffect(JObject json, Card owner, CardEffectTrigger trigger = CardEffectTrigger.PlayCard) : this(json["goal"]?.ToString() ?? "unknown", AmountCalculation.FromJson(json["amount"]), owner, trigger)
    {
    }

    public override void applyEffect()
    {
        // Assuming RoundHandler has a method to apply the effect
        CombatHandler.instance.addGoal(goal, this.amountCalculation.GetValue(this.owner), this.trigger);
    }

    public override CardEffect Clone(Card newOwner)
    {
        return new GoalCardEffect(this.goal, this.amountCalculation, newOwner);
    }

    private string GetDescription(string tableEntry)
    {
        return LocalizationSettings.StringDatabase.GetLocalizedString("CardStrings", tableEntry,
            arguments: new Dictionary<string, object> {
                { "amount", this.amountCalculation.GetValue(this.owner) },
                { "goal", Game.Instance.GetGoalName(this.goal) }
            }
        );
    }

    public override string getDescription()
    {
        return GetDescription("Goal");
    }

    public override string getTriggerDescription()
    {
        return GetDescription("GoalTrigger");
    }
}