using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine.Localization.Settings;

public class DrawCardsEffect : CardEffect {
    private int amount;

    public DrawCardsEffect(int amount) {
        this.amount = amount;
    }

    public DrawCardsEffect(JObject json) {
        this.amount = json["amount"]?.ToObject<int>() ?? 1;
    }

    public override void applyEffect() {
        // Assuming RoundHandler has a method to apply the effect
        CombatHandler.instance.getCardPile().DrawCards(this.amount);
    }

    public override string getDescription() {
        return LocalizationSettings.StringDatabase.GetLocalizedString("CardStrings", "DrawCards",
            arguments: new Dictionary<string, object> {
                { "amount", this.amount },
            }
        );
    }
}