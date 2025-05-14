using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine.Localization.Settings;

public class DiscardCardsEffect : CardEffect {
    private int amount;

    public DiscardCardsEffect(int amount) {
        this.amount = amount;
    }

    public DiscardCardsEffect(JObject json) {
        this.amount = json["amount"]?.ToObject<int>() ?? 1;
    }

    public override void applyEffect() {
        // Assuming RoundHandler has a method to apply the effect
        CombatHandler.instance.getCardPile().DiscardRandomCards(this.amount);
    }

    public override bool canPlay()
    {
        // The card itself is part of the hand, so there needs to be one more card in hand
        return CombatHandler.instance.getCardPile().GetHand().Count >= (amount + 1);
    }

    public override string getDescription() {
        return LocalizationSettings.StringDatabase.GetLocalizedString("CardStrings", "DiscardCards",
            arguments: new Dictionary<string, object> {
                { "amount", this.amount },
            }
        );
    }
}