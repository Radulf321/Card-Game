using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using UnityEngine.Localization.Settings;

public class DrawCardsEffect : CardEffect {
    private int amount;
    private CardEffectTrigger trigger;

    public DrawCardsEffect(int amount, CardEffectTrigger trigger = CardEffectTrigger.PlayCard)
    {
        this.amount = amount;
        this.trigger = trigger;
    }

    public DrawCardsEffect(JObject json, CardEffectTrigger trigger = CardEffectTrigger.PlayCard) : this(amount: json["amount"]?.ToObject<int>() ?? 1, trigger: trigger) {
    }

    public override void applyEffect() {
        // Assuming RoundHandler has a method to apply the effect
        CombatHandler.instance.getCardPile().DrawCards(this.amount, this.trigger);
    }

    public override CardEffect Clone(Card newOwner)
    {
        return new DrawCardsEffect(this.amount, this.trigger);
    }

    public override Task<string> getDescription() {
        return AsyncHelper.HandleToTask(LocalizationSettings.StringDatabase.GetLocalizedStringAsync("CardStrings", "DrawCards",
            arguments: new Dictionary<string, object> {
                { "amount", this.amount },
            }
        ));
    }
}