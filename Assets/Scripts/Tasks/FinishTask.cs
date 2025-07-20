using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using UnityEngine.Localization.Settings;

public class FinishTask : GameTask
{
    public FinishTask(string id, List<Reward> rewards) : base(id, rewards)
    {
    }

    public FinishTask(JObject json) : base(json)
    {
    }

    public override Task<string> GetDescription()
    {
        return AsyncHelper.HandleToTask(LocalizationSettings.StringDatabase.GetLocalizedStringAsync("TaskStrings", "FinishDescription"));
    }

    public override bool IsCompleted()
    {
        return true;
    }
}