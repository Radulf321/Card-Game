using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine.Localization.Settings;

public class GoalCardEffect : CardEffect {

    private Card owner;
    private AmountCalculation amountCalculation;
    private string goal;

    public GoalCardEffect(string goal, AmountCalculation amountCalculation, Card owner)
    {
        this.goal = goal;
        this.amountCalculation = amountCalculation;
        this.owner = owner;
    }

    public GoalCardEffect(string goal, int amount, Card owner) {
        this.goal = goal;
        this.amountCalculation = new ConstantAmountCalculation(amount);
        this.owner = owner;
    }

    public GoalCardEffect(JObject json, Card owner) {
        this.owner = owner;
        this.goal = json["goal"]?.ToString() ?? "unknown";
        this.amountCalculation = AmountCalculation.FromJson(json["amount"]);
    }

    public override void applyEffect() {
        // Assuming RoundHandler has a method to apply the effect
        CombatHandler.instance.addGoal(goal, this.amountCalculation.GetValue(this.owner));
    }

    public override CardEffect Clone(Card newOwner)
    {
        return new GoalCardEffect(this.goal, this.amountCalculation, newOwner);
    }

    public override string getDescription() {
        return LocalizationSettings.StringDatabase.GetLocalizedString("CardStrings", "Goal",
            arguments: new Dictionary<string, object> {
                { "amount", this.amountCalculation.GetValue(this.owner) },
                { "goal", Game.Instance.GetGoalName(this.goal) }
            }
        );
    }
}