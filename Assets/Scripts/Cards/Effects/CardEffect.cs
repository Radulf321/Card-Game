using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

#nullable enable
abstract public class CardEffect {
    public static CardEffect FromJson(JObject json, Card owner, CardEffectTrigger trigger = CardEffectTrigger.PlayCard) {
        string? type = json["type"]?.ToString();
        return type switch
        {
            "goal" => new GoalEffect(json, owner, trigger),
            "energy" => new EnergyEffect(json),
            "drawCards" => new DrawCardsEffect(json),
            "discardCards" => new DiscardCardsEffect(json),
            "caption" => new CaptionEffect(json),
            "afterPlay" => new AfterPlayEffect(json, owner),
            "triggerable" => new TriggerableEffect(json, owner),
            "requirementGoal" => new RequirementGoalEffect(json),
            _ => throw new System.Exception("Invalid card effect type: " + type),
        };
    }
    abstract public void applyEffect();

    abstract public Task<string> getDescription();

    abstract public CardEffect Clone(Card? newOwner);

    virtual public bool canPlay()
    {
        return true;
    }

    virtual public Task<string> getTriggerDescription()
    {
        return this.getDescription();
    }

    virtual public Task<string> getTurnEffectDescription()
    {
        return this.getDescription();
    }
}