using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using UnityEngine.Localization.Settings;

#nullable enable

public class DrawCardsEffect : GameEffect
{
    private int amount;
    private CardEffectTrigger trigger;

    public DrawCardsEffect(int amount, CardEffectTrigger trigger = CardEffectTrigger.PlayCard)
    {
        this.amount = amount;
        this.trigger = trigger;
    }

    public DrawCardsEffect(JObject json, CardEffectTrigger trigger = CardEffectTrigger.PlayCard) : this(amount: json["amount"]?.ToObject<int>() ?? 1, trigger: trigger)
    {
    }

    public override void applyEffect(Enemy? target = null)
    {
        // Assuming RoundHandler has a method to apply the effect
        CombatHandler.instance?.getCardPile().DrawCards(this.amount, this.trigger);
    }

    public override GameEffect Clone(Card? newOwner)
    {
        return new DrawCardsEffect(this.amount, this.trigger);
    }

    private Task<string> GetDescription(string tableEntry)
    {
        return AsyncHelper.HandleToTask(LocalizationSettings.StringDatabase.GetLocalizedStringAsync("CardStrings", tableEntry,
            arguments: new Dictionary<string, object> {
                { "amount", this.amount },
            }
        ));
    }

    public override async Task<string?> GetInternalIconDescription()
    {
        return await GetDescription("DrawCardsIcon");
    }

    public override Task<string> getDescription()
    {
        return GetDescription("DrawCards");
    }
}