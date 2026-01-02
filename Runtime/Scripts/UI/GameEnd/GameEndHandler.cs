using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

#nullable enable
public class GameEndHandler : MonoBehaviour
{
    public TMPro.TMP_FontAsset statisticsFont;
    public GameObject taskOutcomePrefab;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        TaskManager taskManager = Game.Instance.GetTaskManager();
        transform.Find("Background").GetComponent<Image>().sprite = Resources.Load<Sprite>(taskManager.GetGameEndBackground());
        Transform statisticsArea = transform.Find("StatisticsContainer").Find("StatisticsArea");
        Color statisticsColor = taskManager.GetStatisticsColor();
        statisticsColor.a = 0.7f;
        statisticsArea.GetComponent<Image>().color = statisticsColor;
        Transform resultsArea = statisticsArea.Find("ResultsScroll").Find("Viewport").Find("ResultsArea");
        for (int i = 0; i < resultsArea.childCount; i++)
        {
            Destroy(resultsArea.GetChild(i).gameObject);
        }

        GameObject totalWins = new GameObject("TotalWins");
        totalWins.transform.SetParent(resultsArea, true);

        TMPro.TextMeshProUGUI totalWinsText = totalWins.AddComponent<TMPro.TextMeshProUGUI>();

        _ = AsyncHelper.UpdateTextFromTask(totalWinsText, AsyncHelper.HandleToTask(
            LocalizationSettings.StringDatabase.GetLocalizedStringAsync("UIStrings", "TotalWins",
            arguments: new Dictionary<string, object> {
                { "amount", taskManager.GetTotalWins() }
            }
        )
        ));
        totalWinsText.fontSize = 64; // Set font size or other properties as needed
        totalWinsText.color = Color.black; // Set color if desired
        totalWinsText.font = statisticsFont;

        LayoutElement totalWinsLayout = totalWins.AddComponent<LayoutElement>();
        totalWinsLayout.preferredHeight = 100;

        GameObject tasksPadding = new GameObject("TasksPadding");
        tasksPadding.transform.SetParent(resultsArea, true);

        LayoutElement tasksPaddingLayout = tasksPadding.AddComponent<LayoutElement>();
        tasksPaddingLayout.preferredHeight = 50;

        foreach (GameTask task in taskManager.GetActiveTasks())
        {
            GameObject taskOutcome = Instantiate(taskOutcomePrefab, resultsArea);
            TaskOutcomeHandler outcomeHandler = taskOutcome.GetComponent<TaskOutcomeHandler>();
            outcomeHandler.SetTask(task);
        }
        
        Game.Instance.SendTriggerMessage(new TriggerMessage(TriggerType.EndGame));
    }

    public void ReturnToMainMenu()
    {
        FadeHandler.Instance!.LoadScene("MainMenuScene");
    }

    // Update is called once per frame
    void Update()
    {
    }
}
