using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using UnityEngine.Localization.Settings;

public class WinsTask : GameTask
{
    private int amount;

    public WinsTask(string id, List<Reward> rewards, int amount) : base(id, rewards)
    {
        this.amount = amount;
    }

    public WinsTask(JObject json) : base(json)
    {
        this.amount = json["amount"]!.ToObject<int>();
    }

    public override Task<string> GetDescription()
    {
        return AsyncHelper.HandleToTask(LocalizationSettings.StringDatabase.GetLocalizedStringAsync("TaskStrings", "WinDescription",
            arguments: new Dictionary<string, object> {
                { "amount", this.amount }
            }
        ));
    }

    public override int? GetProgress()
    {
        return Game.Instance.GetTaskManager().GetTotalWins();
    }

    public override int? GetTotal()
    {
        return this.amount;
    }
}