using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;

#nullable enable

public class TaskManager
{
    private const int MAX_ACTIVE_TASKS = 3;
    private const string TASK_COMPLETE_PREFIX = "TaskComplete_";

    private List<GameTask> tasks;
    private List<GameTask> activeTasks;
    private Color statisticsColor;
    private string gameEndBackground;

    // Statistics relevant for tasks
    private Dictionary<string, int> wins;

    public TaskManager()
    {
        this.wins = new Dictionary<string, int>();
        this.tasks = new List<GameTask>();
        this.activeTasks = new List<GameTask>();
        this.statisticsColor = Color.black;
        this.gameEndBackground = "";
    }

    public TaskManager(string resourcePath, JObject json) : this()
    {
        this.statisticsColor = ColorUtility.TryParseHtmlString("#" + json["statisticsColor"]!.ToString(), out Color color) ? color : Color.black;
        this.gameEndBackground = json["gameEndBackground"]!.ToString();
        JArray tasksData = JArray.Parse(Resources.Load<TextAsset>(resourcePath + "/Tasks").text);
        foreach (JObject taskJson in tasksData)
        {
            this.tasks.Add(GameTask.FromJson(taskJson));
        }
    }

    public void Initialize()
    {
        Game.Instance.SubscribeToTriggerMessages(OnTriggerMessage);
        foreach (GameTask task in this.tasks)
        {
            if (IsTaskCompleted(task))
            {
                task.CollectRewards();
            }
            else if ((this.activeTasks.Count < MAX_ACTIVE_TASKS) && task.IsAvailable())
            {
                this.activeTasks.Add(task);
            }
        }
    }

    public JObject SaveToJson()
    {
        JArray tasksData = new JArray();
        foreach (GameTask task in this.activeTasks)
        {
            JObject taskData = task.SaveToJson();
            taskData["id"] = task.GetID();
            tasksData.Add(taskData);
        }
        JObject winsData = new JObject();
        foreach (KeyValuePair<string, int> pair in this.wins)
        {
            winsData[pair.Key] = pair.Value;
        }
        return new JObject {
            ["wins"] = winsData,
            ["tasks"] = tasksData
        };
    }

    public void LoadFromJson(JObject json)
    {
        this.wins = new Dictionary<string, int>();
        if (json["wins"] != null)
        {
            foreach (JProperty property in json["wins"]!.ToObject<JObject>()?.Properties() ?? new List<JProperty>())
            {
                this.wins[property.Name] = property.Value.ToObject<int>();
            }
        }
        if (json["tasks"] != null)
        {
            foreach (JObject taskData in json["tasks"]!.ToObject<List<JObject>>() ?? new List<JObject>())
            {
                GameTask? task = this.tasks.Find(task => task.GetID() == taskData["id"]!.ToString());
                if (task != null)
                {
                    task.LoadFromJson(taskData);
                    this.activeTasks.Add(task);
                }
            }
        }
    }

    public void EndGame()
    {
        bool needSave = false;
        foreach (GameTask task in this.activeTasks)
        {
            if (task.IsCompleted())
            {
                PlayerPrefs.SetInt(GetTaskCompleteKey(task), 1);
                needSave = true;
            }
        }
        if (needSave)
        {
            PlayerPrefs.Save();
        }
        Game.Instance.UnsubscribeFromTriggerMessages(OnTriggerMessage);
    }

    public int GetTotalWins()
    {
        int totalWins = 0;
        foreach (int win in wins.Values)
        {
            totalWins += win;
        }
        return totalWins;
    }

    public int GetWins(string combatTargetID)
    {
        if (wins.ContainsKey(combatTargetID))
        {
            return wins[combatTargetID];
        }
        else
        {
            return 0;
        }
    }

    public List<GameTask> GetActiveTasks()
    {
        return new List<GameTask>(this.activeTasks);
    }

    public Color GetStatisticsColor()
    {
        return this.statisticsColor;
    }

    public string GetGameEndBackground()
    {
        return Game.Instance.GetResourcePath() + "/Graphics/Backgrounds/" + this.gameEndBackground;
    }

    public bool IsTaskCompleted(GameTask task)
    {
        return IsTaskCompleted(task.GetID());
    }

    public bool IsTaskCompleted(string taskID)
    {
        return PlayerPrefs.GetInt(GetTaskCompleteKey(taskID), 0) == 1;
    }

    public void DebugCompleteAllTasks()
    {
        foreach (GameTask task in this.tasks)
        {
            PlayerPrefs.SetInt(GetTaskCompleteKey(task), 1);
        }
        PlayerPrefs.Save();
    }

    private void OnTriggerMessage(TriggerMessage message)
    {
        if (message.GetTriggerType() == TriggerType.EndCombat)
        {
            string? combatTargetID = message.GetData().GetCombatTarget()?.GetID();
            if (combatTargetID == null)
            {
                throw new System.ArgumentNullException("CombatTarget ID cannot be null.");
            }
            if (!wins.ContainsKey(combatTargetID))
            {
                wins[combatTargetID] = 1;
            }
            else
            {
                wins[combatTargetID]++;
            }
        }
    }

    private string GetTaskCompleteKey(GameTask task, string? resourcePath = null)
    {
        return GetTaskCompleteKey(task.GetID(), resourcePath);
    }

    private string GetTaskCompleteKey(string taskID, string? resourcePath = null)
    {
        return (resourcePath ?? Game.Instance.GetResourcePath()) + TASK_COMPLETE_PREFIX + taskID;
    }
}