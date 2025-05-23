using Newtonsoft.Json.Linq;
using UnityEngine.Localization.Settings;

public class CardReward : Reward {
    private Card card;

    public CardReward(Card card) {
        this.card = card;
    }

    public CardReward(JObject rewardData) {
        this.card = Game.Instance.GetCard(rewardData["card"]?.ToString() ?? "Undefined");
    }

    public Card GetCard() {
        return card;
    }
    public override void Collect() {
        Game.Instance.GetPlayer().AddCardToDeck(this.GetCard());
    }
    public override string ToNiceString() {
        return LocalizationSettings.StringDatabase.GetLocalizedString("RewardStrings", "CardShort");
    }
}