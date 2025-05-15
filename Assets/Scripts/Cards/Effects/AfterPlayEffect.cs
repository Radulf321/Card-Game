using Newtonsoft.Json.Linq;

public class AfterPlayEffect : CardEffect {
    private CardAfterPlay afterPlay;
    private Card owner;

    public AfterPlayEffect(JObject json, Card owner) {
        this.afterPlay = CardAfterPlayHelper.FromJson(json["afterPlay"]);
        this.owner = owner;
    }

    public override void applyEffect() {
        this.owner.SetAfterPlay(this.afterPlay);
    }

    public override string getDescription() {
        return "TODO";
    }
}