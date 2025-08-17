using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

#nullable enable
public class GameEndHandler : MonoBehaviour
{
    public GameObject taskOutcomePrefab;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        TaskManager taskManager = Game.Instance.GetTaskManager();
        transform.Find("Background").GetComponent<Image>().sprite = Resources.Load<Sprite>(taskManager.GetGameEndBackground());
        Transform statisticsArea = transform.Find("StatisticsContainer").Find("StatisticsArea");
        statisticsArea.GetComponent<Image>().sprite = Resources.Load<Sprite>(taskManager.GetStatisticsBackground());
        Transform tasksArea = statisticsArea.Find("TasksArea");
        for (int i = 0; i < tasksArea.childCount; i++)
        {
            Destroy(tasksArea.GetChild(i).gameObject);
        }
        foreach (GameTask task in taskManager.GetActiveTasks())
        {
            GameObject taskOutcome = Instantiate(taskOutcomePrefab, tasksArea);
            TaskOutcomeHandler outcomeHandler = taskOutcome.GetComponent<TaskOutcomeHandler>();
            outcomeHandler.SetTask(task);
        }

        Transform basicStatisticsArea = statisticsArea.Find("BasicStatisticsArea");
        GameObject totalWins = new GameObject("TotalWins");
        totalWins.transform.SetParent(basicStatisticsArea, true);

        TMPro.TextMeshProUGUI totalWinsText = totalWins.AddComponent<TMPro.TextMeshProUGUI>();

        AsyncHelper.UpdateTextFromTask(totalWinsText, AsyncHelper.HandleToTask(
            LocalizationSettings.StringDatabase.GetLocalizedStringAsync("UIStrings", "TotalWins",
            arguments: new Dictionary<string, object> {
                { "amount", taskManager.GetTotalWins() }
            }
        )
        ));
        totalWinsText.fontSize = 36; // Set font size or other properties as needed
        totalWinsText.color = Color.black; // Set color if desired
        
        Game.Instance.SendTriggerMessage(new TriggerMessage(TriggerType.EndGame));
    }

    public void ReturnToMainMenu()
    {
        FadeHandler.Instance!.LoadScene("DebugScene");
    }

    // Update is called once per frame
    void Update()
    {
    }
}
