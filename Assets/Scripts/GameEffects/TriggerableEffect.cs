using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using UnityEngine.Localization.Settings;

#nullable enable

public enum LimitType
{
    PerTurn,
    Total,
}

public class TriggerableEffect : GameEffect
{
    private TriggerAction triggerAction;
    private Card owner;
    private int? limit;
    private LimitType? limitType;
    private int? currentLimit;


    public TriggerableEffect(TriggerAction triggerAction, Card owner, LimitType? limitType = null, int? limit = null)
    {
        this.triggerAction = triggerAction;
        this.owner = owner;
        this.limit = limit;
        this.limitType = limitType;
    }

    public TriggerableEffect(JObject json, Card owner) : this(
        triggerAction: new TriggerAction(json["action"] as JObject ?? new JObject(), owner: owner),
        owner: owner,
        limitType: EnumHelper.ParseEnum<LimitType>(json["limitType"]?.ToString() ?? "InvalidType"),
        limit: json["limit"]?.ToObject<int>()
    )
    {
    }

    public override void applyEffect()
    {
        this.triggerAction.Subscribe();
        this.triggerAction.SetOnTrigger(this.OnTrigger);
        if (this.limitType == LimitType.PerTurn)
        {
            Game.Instance.SubscribeToTriggerMessages(this.HandleMessage);
        }
        this.currentLimit = limit;
        this.owner.SetAfterPlay(CardAfterPlay.StayInPlay);
    }

    public override GameEffect Clone(Card? newOwner)
    {
        return new TriggerableEffect(triggerAction.Clone(newOwner), newOwner ?? this.owner, limitType: this.limitType, limit: this.limit);
    }

    public override async Task<string> getDescription()
    {
        string conditionDescription = await this.triggerAction.GetTriggerDescription();
        string effectDescription = await this.triggerAction.GetEffectDescription();
        return await AsyncHelper.HandleToTask(LocalizationSettings.StringDatabase.GetLocalizedStringAsync("CardStrings", "Triggerable",
            arguments: new Dictionary<string, object?> {
                { "condition", conditionDescription },
                { "effect", effectDescription },
                { "limitType", this.limitType },
                { "limit", this.limit }
            }
        ));
    }

    private void OnTrigger()
    {
        if (this.currentLimit != null)
        {
            this.currentLimit--;
        }
        // This discards the card from play if limit is exhausted and limit is total, i.e., does not refresh
        // TODO: Consider permanent cards with different effects and limits. In case limit is exhausted
        // does not mean it's exhausted for all effects, so you should check if card can be discarded
        if ((this.currentLimit == 0) && (this.limitType == LimitType.Total))
        {
            Game.Instance.UnsubscribeFromTriggerMessages(HandleMessage);
            CombatHandler.instance?.getCardPile().DiscardCard(this.owner);
        }
    }

    private void HandleMessage(TriggerMessage triggerMessage)
    {
        switch (triggerMessage.GetTriggerType())
        {
            case TriggerType.StartTurn:
                this.currentLimit = this.limit;
                break;

            case TriggerType.EndRound:
                Game.Instance.UnsubscribeFromTriggerMessages(HandleMessage);
                break;


        }
        
    }
}