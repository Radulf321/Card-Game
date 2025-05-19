using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.Localization.Settings;

#nullable enable

public enum LimitType
{
    PerTurn,
    Total,
}

public class TriggerableEffect : CardEffect
{
    private TriggerMessageCondition condition;
    private CardEffect triggerEffect;
    private Card owner;
    private int? limit;
    private LimitType? limitType;
    private int? currentLimit;


    public TriggerableEffect(TriggerMessageCondition condition, CardEffect triggerEffect, Card owner, LimitType? limitType = null, int? limit = null)
    {
        this.condition = condition;
        this.triggerEffect = triggerEffect;
        this.owner = owner;
        this.limit = limit;
        this.limitType = limitType;
    }

    public TriggerableEffect(JObject json, Card owner) : this(
        condition: TriggerMessageCondition.FromJson(json["condition"] as JObject),
        triggerEffect: CardEffect.FromJson(json["effect"] as JObject ?? new JObject(), owner, CardEffectTrigger.TriggerEffect),
        owner: owner,
        limitType: EnumHelper.ParseEnum<LimitType>(json["limitType"]?.ToString() ?? "InvalidType"),
        limit: json["limit"]?.ToObject<int>()
    )
    {
    }

    public override void applyEffect()
    {
        CombatHandler.instance.SubscribeToTriggerMessages(this.HandleMessage);
        this.currentLimit = limit;
        this.owner.SetAfterPlay(CardAfterPlay.StayInPlay);
    }

    public override CardEffect Clone(Card? newOwner)
    {
        return new TriggerableEffect(this.condition, this.triggerEffect.Clone(newOwner), newOwner ?? this.owner, limitType: this.limitType, limit: this.limit);
    }

    public override string getDescription()
    {
        Debug.Log(this.limitType);
        Debug.Log(this.limit);
        return LocalizationSettings.StringDatabase.GetLocalizedString("CardStrings", "Triggerable",
            arguments: new Dictionary<string, object?> {
                { "condition", this.condition.GetDescription() },
                { "effect", this.triggerEffect.getTriggerDescription() },
                { "limitType", this.limitType },
                { "limit", this.limit }
            }
        );
    }

    private void HandleMessage(TriggerMessage triggerMessage)
    {
        if ((triggerMessage.GetTriggerType() == TriggerType.StartTurn) && (this.limitType == LimitType.PerTurn))
        {
            this.currentLimit = this.limit;
        }
        if (this.condition.FulfillsCondition(triggerMessage) && ((this.currentLimit ?? 42) > 0))
        {
            this.triggerEffect.applyEffect();
            if (this.currentLimit != null)
            {
                this.currentLimit--;
            }
            // This discards the card from play if limit is exhausted and limit is total, i.e., does not refresh
            // TODO: Consider permanent cards with different effects and limits. In case limit is exhausted
            // does not mean it's exhausted for all effects, so you should check if card can be discarded
            if ((this.currentLimit == 0) && (this.limitType == LimitType.Total))
            {
                CombatHandler.instance.UnsubscribeFromTriggerMessages(HandleMessage);
                CombatHandler.instance.getCardPile().DiscardCard(this.owner);
            }
        }
    }
}