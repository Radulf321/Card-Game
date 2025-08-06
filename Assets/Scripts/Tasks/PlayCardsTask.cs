using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.Localization.Settings;

#nullable enable

public class PlayCardsTask : GameTask
{
    private int amount;

    private int progress;
    private int maxProgress;

    public PlayCardsTask(string id, List<Reward> rewards, int amount, string? combatTargetID = null) : base(id, rewards)
    {
        this.amount = amount;
        this.progress = 0;
        this.maxProgress = 0;
        Game.Instance.SubscribeToTriggerMessages(this.HandleMessage);
    }

    public PlayCardsTask(JObject json) : base(json)
    {
        this.amount = json["amount"]!.ToObject<int>();
        this.progress = 0;
        this.maxProgress = 0;
        Game.Instance.SubscribeToTriggerMessages(this.HandleMessage);
    }

    public override Task<string> GetDescription()
    {
        return AsyncHelper.HandleToTask(LocalizationSettings.StringDatabase.GetLocalizedStringAsync("TaskStrings", "PlayCardsDescription",
            arguments: new Dictionary<string, object?> {
                { "amount", this.amount }
            }
        ));
    }

    public override int? GetProgress()
    {
        return this.maxProgress;
    }

    public override int? GetTotal()
    {
        return this.amount;
    }

    private void HandleMessage(TriggerMessage triggerMessage)
    {
        switch (triggerMessage.GetTriggerType())
        {
            case TriggerType.PlayCard:
                this.progress++;
                this.maxProgress = Mathf.Max(this.maxProgress, this.progress);
                if (this.maxProgress >= this.amount)
                {
                    Game.Instance.UnsubscribeFromTriggerMessages(this.HandleMessage);
                }
                break;

            case TriggerType.StartTurn:
                this.progress = 0;
                break;

            default:
                break;
        }

    }
}