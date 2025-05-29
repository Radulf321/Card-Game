using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using UnityEngine.Localization.Settings;

public class GoalEffect : CardEffect
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

    public GoalEffect(JObject json, Card owner, CardEffectTrigger trigger = CardEffectTrigger.PlayCard) : this(json["goal"]?.ToString() ?? "unknown", AmountCalculation.FromJson(json["amount"]), owner, trigger)
    {
    }

    public override void applyEffect()
    {
        // Assuming RoundHandler has a method to apply the effect
        CombatHandler.instance.addGoal(goal, this.amountCalculation.GetValue(this.owner), this.trigger);
    }

    public override CardEffect Clone(Card newOwner)
    {
        return new GoalEffect(this.goal, this.amountCalculation, newOwner, this.trigger);
    }

    private Task<string> GetDescription(string tableEntry)
    {
        return AsyncHelper.HandleToTask(LocalizationSettings.StringDatabase.GetLocalizedStringAsync("CardStrings", tableEntry,
            arguments: new Dictionary<string, object> {
                { "amount", this.amountCalculation.GetValue(this.owner) },
                { "goal", Game.Instance.GetGoalName(this.goal) }
            }
        ));
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