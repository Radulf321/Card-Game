using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

#nullable enable
public class DialogReward : Dialog
{
    private List<Reward> rewards;

    public DialogReward(List<Reward> rewards, Dialog? nextDialog = null) : base(nextDialog)
    {
        this.rewards = rewards;
    }

    public DialogReward(JObject dialogData, Dialog? nextDialog = null) : this(Reward.FromJson(dialogData["rewards"] as JArray), nextDialog) { }

    public List<Reward> GetRewards()
    {
        return this.rewards;
    }

    public override async Task ShowDialog()
    {
        foreach (Reward reward in this.rewards)
        {
            reward.Collect();
        }
        await DialogHandler.Instance!.ShowReward(this);
        await base.ShowDialog();
    }
}