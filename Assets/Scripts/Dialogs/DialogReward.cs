using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

#nullable enable
public class DialogReward : Dialog
{
    private List<Reward> rewards;
    private Action onFinish;

    public DialogReward(List<Reward> rewards, Action onFinish)
    {
        this.rewards = rewards;
        this.onFinish = onFinish;
    }

    public DialogReward(List<Reward> rewards, Dialog nextDialog) : this(rewards, nextDialog.ShowDialog) { }

    public DialogReward(JObject dialogData, Action onFinish) : this(Reward.FromJson(dialogData["rewards"] as JArray), onFinish) { }

    public Action GetOnFinish()
    {
        return onFinish;
    }

    public List<Reward> GetRewards()
    {
        return this.rewards;
    }

    public override void ShowDialog()
    {
        foreach (Reward reward in this.rewards)
        {
            reward.Collect();
        }
        UnityEngine.Object.FindAnyObjectByType<DialogHandler>().ShowReward(this);
    }
}