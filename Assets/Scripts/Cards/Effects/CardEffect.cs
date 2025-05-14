using Newtonsoft.Json.Linq;

#nullable enable
abstract public class CardEffect {
    public static CardEffect FromJson(JObject json, Card owner) {
        string? type = json["type"]?.ToString();
        switch (type) {
            case "goal":
                return new GoalCardEffect(json, owner);
            case "energy":
                return new EnergyEffect(json);
            case "drawCards":
                return new DrawCardsEffect(json);
            case "discardCards":
                return new DiscardCardsEffect(json);
            case "caption":
                return new CaptionEffect(json);
            default:
                throw new System.Exception("Invalid card effect type: " + type);
        }
    }
    abstract public void applyEffect();

    abstract public string getDescription();

    virtual public bool canPlay() {
        return true;
    }
}