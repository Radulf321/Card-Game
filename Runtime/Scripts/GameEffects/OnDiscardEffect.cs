using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using UnityEngine.Localization.Settings;

#nullable enable

public class OnDiscardEffect : GameEffect
{
    private GameEffect effect;
    private Card owner;


    public OnDiscardEffect(GameEffect effect, Card? owner)
    {
        this.effect = effect;
        if (owner == null)
        {
            throw new ArgumentException("Owner must not be null for OnDiscardEffect");
        }
        this.owner = owner;
    }

    public OnDiscardEffect(JObject json, Card? owner) : this(
        effect: GameEffect.FromJson((json["effect"] as JObject)!, owner: owner, trigger: CardEffectTrigger.Discard),
        owner: owner
    )
    {
    }

    public override void applyEffect(Enemy? target = null)
    {
        // No effect when card is played
    }

    public override GameEffect Clone(Card? newOwner)
    {
        return new OnDiscardEffect(effect.Clone(newOwner), newOwner ?? this.owner);
    }

    public override void OnDraw()
    {
        Game.Instance.SubscribeToTriggerMessages(this.HandleMessage);
    }

    public override async Task<string> getDescription()
    {
        string effectDescription = await this.effect.getTriggerDescription();
        return await AsyncHelper.HandleToTask(LocalizationSettings.StringDatabase.GetLocalizedStringAsync("CardStrings", "OnDiscard",
            arguments: new Dictionary<string, object?> {
                { "effect", effectDescription },
            }
        ));
    }

    public override async Task<string?> GetInternalIconDescription()
    {
        string? effectDescription = await this.effect.GetInternalIconDescription();
        if (effectDescription == null)
        {
            return null;
        }
        return await AsyncHelper.HandleToTask(LocalizationSettings.StringDatabase.GetLocalizedStringAsync("CardStrings", "OnDiscardIcon",
            arguments: new Dictionary<string, object> {
                { "effect", effectDescription },
            }
        ));
    }

    private void HandleMessage(TriggerMessage triggerMessage)
    {
        if ((triggerMessage.GetTriggerType() == TriggerType.DiscardCard) && (triggerMessage.GetData().GetCard() == this.owner))
        {
            Game.Instance.UnsubscribeFromTriggerMessages(this.HandleMessage);
            if (triggerMessage.GetData().GetDiscardType() == DiscardType.Discard)
            {
                effect.applyEffect();
            }
        }

    }
}