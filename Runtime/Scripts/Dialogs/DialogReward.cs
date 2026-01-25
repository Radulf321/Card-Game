using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

#nullable enable
public class DialogReward : Dialog
{
    private List<Reward> rewards;

    public DialogReward(List<Reward> rewards, Dialog? nextDialog = null, string? id = null) : base(nextDialog: nextDialog, id: id)
    {
        this.rewards = rewards;
    }

    public DialogReward(JObject dialogData, Dialog? nextDialog = null) : this(
        Reward.FromJson(dialogData["rewards"] as JArray),
        nextDialog,
        Dialog.GetIDFromJson(dialogData)
    ) { }

    public List<Reward> GetRewards()
    {
        return this.rewards;
    }

    public override async Task ShowDialog()
    {
        await DialogHandler.Instance!.ShowReward(this);
        bool previousGameLostState = Game.Instance!.IsGameLost();
        foreach (Reward reward in this.rewards)
        {
            reward.Collect();
        }
        // If the game is now lost, completely abort the dialog
        // Loosing will be handled elsewhere
        if (!previousGameLostState && Game.Instance!.IsGameLost())
        {
            DialogHandler.Instance!.SkipNextOnFinish();
            return;
        }
        await base.ShowDialog();
    }
}