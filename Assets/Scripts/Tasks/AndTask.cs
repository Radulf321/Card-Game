using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using UnityEngine.Localization.Settings;

#nullable enable
public class AndTask : GameTask
{
    private List<GameTask> tasks;

    public AndTask(string id, List<Reward> rewards, List<GameTask> tasks) : base(id, rewards)
    {
        this.tasks = tasks;
    }

    public AndTask(JObject json) : base(json)
    {
        List<GameTask> tasks = new List<GameTask>();
        foreach (JObject taskJson in (json["tasks"] as JArray) ?? new JArray())
        {
            tasks.Add(GameTask.FromJson(taskJson));
        }
        this.tasks = tasks;
    }

    public async override Task<string> GetDescription()
    {
        List<string> taskDescriptions = new List<string>();
        foreach (GameTask task in this.tasks)
        {
            taskDescriptions.Add(await task.GetDescription());
        }
        return await AsyncHelper.HandleToTask(LocalizationSettings.StringDatabase.GetLocalizedStringAsync("TaskStrings", "AndDescription",
            arguments: new Dictionary<string, object> {
                { "tasks", taskDescriptions }
            }
        ));
    }

    public override int? GetProgress()
    {
        int totalProgress = 0;
        foreach (GameTask task in this.tasks)
        {
            totalProgress += task.GetProgress() ?? (task.IsCompleted() ? 1 : 0);
        }
        return totalProgress;
    }

    public override int? GetTotal()
    {
        int total = 0;
        foreach (GameTask task in this.tasks)
        {
            total += task.GetTotal() ?? 1;
        }
        return total;
    }
}