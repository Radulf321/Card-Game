using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine.Localization.Settings;

#nullable enable

public class TriggerableEffect : CardEffect
{
    private TriggerMessageCondition condition;
    private CardEffect triggerEffect;
    private Card owner;


    public TriggerableEffect(TriggerMessageCondition condition, CardEffect triggerEffect, Card owner)
    {
        this.condition = condition;
        this.triggerEffect = triggerEffect;
        this.owner = owner;
    }

    public TriggerableEffect(JObject json, Card owner) : this(TriggerMessageCondition.FromJson(json["condition"] as JObject), CardEffect.FromJson(json["effect"] as JObject ?? new JObject(), owner, CardEffectTrigger.TriggerEffect), owner)
    {

    }

    public override void applyEffect()
    {
        CombatHandler.instance.SubscribeToTriggerMessages(this.HandleMessage);
        this.owner.SetAfterPlay(CardAfterPlay.StayInPlay);
    }

    public override CardEffect Clone(Card? newOwner)
    {
        return new TriggerableEffect(this.condition, this.triggerEffect.Clone(newOwner), newOwner ?? this.owner);
    }

    public override string getDescription()
    {
        return LocalizationSettings.StringDatabase.GetLocalizedString("CardStrings", "Triggerable",
            arguments: new Dictionary<string, object> {
                { "condition", this.condition.GetDescription() },
                { "effect", this.triggerEffect.getTriggerDescription() }
            }
        );
    }

    private void HandleMessage(TriggerMessage triggerMessage)
    {
        if (this.condition.FulfillsCondition(triggerMessage))
        {
            this.triggerEffect.applyEffect();
        }
    }
}