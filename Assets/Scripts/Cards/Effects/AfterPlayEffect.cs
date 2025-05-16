using Newtonsoft.Json.Linq;

#nullable enable
public class AfterPlayEffect : CardEffect {
    private CardAfterPlay afterPlay;
    private Card owner;

    public AfterPlayEffect(CardAfterPlay afterPlay, Card owner)
    {
        this.afterPlay = afterPlay;
        this.owner = owner;
    }

    public AfterPlayEffect(JObject json, Card owner)
    {
        this.afterPlay = CardAfterPlayHelper.FromJson(json["afterPlay"]!);
        this.owner = owner;
    }

    public override void applyEffect() {
        this.owner.SetAfterPlay(this.afterPlay);
    }

    public override CardEffect Clone(Card? newOwner)
    {
        return new AfterPlayEffect(this.afterPlay, newOwner ?? this.owner);
    }

    public override string getDescription() {
        return "TODO";
    }
}