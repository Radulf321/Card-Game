using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using UnityEngine.Localization.Settings;

#nullable enable

public class TriggerAction
{
    private TriggerMessageCondition trigger;
    private GameEffect effect;
    private List<StatusCondition> conditions;
    private Action? onTrigger;
    private bool unregisterOnEndRound = false;


    public TriggerAction(TriggerMessageCondition trigger, GameEffect effect, List<StatusCondition>? conditions = null, bool unregisterOnEndRound = true)
    {
        this.trigger = trigger;
        this.effect = effect;
        this.conditions = conditions ?? new List<StatusCondition>();
        this.unregisterOnEndRound = unregisterOnEndRound;
    }

    public TriggerAction(JObject json, Card? owner = null, bool? unregisterOnEndRound = null) : this(
        trigger: TriggerMessageCondition.FromJson((json["trigger"] as JObject)!),
        effect: GameEffect.FromJson(json["effect"] as JObject ?? new JObject(), owner, CardEffectTrigger.TriggerEffect),
        unregisterOnEndRound: unregisterOnEndRound ?? json["unregisterOnEndRound"]?.ToObject<bool>() ?? true
    )
    {
        List<StatusCondition> conditions = new List<StatusCondition>();
        if (json["conditions"] is JArray conditionsArray)
        {
            foreach (JToken conditionJson in conditionsArray)
            {
                conditions.Add(StatusCondition.FromJson(conditionJson as JObject ?? new JObject()));
            }
        }
        this.conditions = conditions;
    }

    public void Subscribe()
    {
        Game.Instance.SubscribeToTriggerMessages(this.HandleMessage);
    }

    public void Unsubscribe()
    {
        Game.Instance.UnsubscribeFromTriggerMessages(this.HandleMessage);
    }

    public void SetOnTrigger(Action onTrigger)
    {
        this.onTrigger = onTrigger;
    }

    public TriggerAction Clone(Card? newOwner)
    {
        return new TriggerAction(
            trigger: this.trigger,
            effect: this.effect.Clone(newOwner),
            conditions: new List<StatusCondition>(this.conditions)
        );
    }

    public Task<string> GetTriggerDescription(LimitType? limitType = null, int? limit = null)
    {
        return this.trigger.GetFullDescription(limitType, limit);
    }

    public async Task<string?> GetIconDescription()
    {
        string? triggerDescription = await this.trigger.GetIconDescription();
        string? effectDescription = await this.effect.GetInternalIconDescription();
        if ((triggerDescription == null) || (effectDescription == null))
        {
            return null;
        }

        return await AsyncHelper.HandleToTask(LocalizationSettings.StringDatabase.GetLocalizedStringAsync("CardStrings", "TriggerIcon",
            arguments: new Dictionary<string, object> {
                { "trigger", triggerDescription },
                { "effect", effectDescription }
            }
        ));
    }

    public Task<string> GetEffectDescription()
    {
        return this.effect.getTriggerDescription();
    }

    public Task<string?> GetEffectIconDescription()
    {
        return this.effect.GetInternalIconDescription();
    }

    private void HandleMessage(TriggerMessage triggerMessage)
    {
        if (this.trigger.FulfillsCondition(triggerMessage) &&
            this.conditions.TrueForAll(condition => condition.IsFulfilled()))
        {
            this.effect.applyEffect();
            this.onTrigger?.Invoke();
        }

        if (this.unregisterOnEndRound && (triggerMessage.GetTriggerType() == TriggerType.EndRound))
        {
            this.Unsubscribe();
        }
    }
}