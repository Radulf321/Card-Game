using Newtonsoft.Json.Linq;

#nullable enable
abstract public class CardEffect {
    public static CardEffect FromJson(JObject json, Card owner) {
        string? type = json["type"]?.ToString();
        return type switch
        {
            "goal" => new GoalCardEffect(json, owner),
            "energy" => new EnergyEffect(json),
            "drawCards" => new DrawCardsEffect(json),
            "discardCards" => new DiscardCardsEffect(json),
            "caption" => new CaptionEffect(json),
            "afterPlay" => new AfterPlayEffect(json, owner),
            _ => throw new System.Exception("Invalid card effect type: " + type),
        };
    }
    abstract public void applyEffect();

    abstract public string getDescription();

    virtual public bool canPlay() {
        return true;
    }
}