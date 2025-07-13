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

    // Statistics relevant for tasks
    private int totalWins;

    public TaskManager()
    {
        totalWins = 0;
        this.tasks = new List<GameTask>();
        this.activeTasks = new List<GameTask>();
    }

    public TaskManager(string resourcePath) : this()
    {
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
            bool taskComplete = PlayerPrefs.GetInt(GetTaskCompleteKey(task), 0) == 1;
            if (taskComplete)
            {
                task.CollectRewards();
            }
            else if (this.activeTasks.Count < MAX_ACTIVE_TASKS)
            {
                this.activeTasks.Add(task);
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
        return totalWins;
    }

    private void OnTriggerMessage(TriggerMessage message)
    {
        if (message.GetTriggerType() == TriggerType.EndCombat)
        {
            if (message.GetData().GetSuccess() == true)
            {
                totalWins++;
            }
        }
    }

    private string GetTaskCompleteKey(GameTask task, string? resourcePath = null)
    {
        return (resourcePath ?? Game.Instance.GetResourcePath()) + TASK_COMPLETE_PREFIX + task.GetID();
    }
}