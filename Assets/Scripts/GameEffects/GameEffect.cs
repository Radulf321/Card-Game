using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

#nullable enable
abstract public class GameEffect
{
    public static GameEffect FromJson(JObject json, Card? owner = null, CardEffectTrigger trigger = CardEffectTrigger.PlayCard)
    {
        string? type = json["type"]?.ToString();
        return type switch
        {
            "goal" => new GoalEffect(json, owner, trigger),
            "energy" => new EnergyEffect(json),
            "drawCards" => new DrawCardsEffect(json, trigger),
            "discardCards" => new DiscardCardsEffect(json),
            "caption" => new CaptionEffect(json),
            "afterPlay" => new AfterPlayEffect(json, owner!),
            "triggerable" => new TriggerableEffect(json, owner!),
            "requirementGoal" => new RequirementGoalEffect(json),
            "dialog" => new DialogEffect(json),
            "onDiscard" => new OnDiscardEffect(json, owner),
            "playOwner" => new PlayOwnerEffect(json, owner),
            _ => throw new System.Exception("Invalid card effect type: " + type),
        };
    }
    abstract public void applyEffect();

    abstract public Task<string> getDescription();

    abstract public GameEffect Clone(Card? newOwner);

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

    virtual public void OnDraw()
    {
        // Usually nothing happens when a card is drawn, but some may want to prepare or trigger an effect
    }
}