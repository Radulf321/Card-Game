using Newtonsoft.Json.Linq;

public abstract class Reward {
    public static Reward FromJson(JObject json) {
        string type = json["type"]?.ToString() ?? "Undefined";
        switch (type) {
            case "card":
                return new CardReward(json);
            default:
                throw new System.Exception("Reward type not recognized: " + type);
        }
    }
    abstract public void Collect();
    abstract public string ToNiceString();
}