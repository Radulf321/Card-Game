using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

#nullable enable
public class TaskOutcomeHandler : MonoBehaviour
{
    private GameTask? task;
    bool needUpdate = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    async void Update()
    {
        if (needUpdate)
        {
            await UpdateDescription();
            Transform rewardArea = transform.Find("RewardDisplayArea");
            bool completed = task?.IsCompleted() ?? false;
            rewardArea.gameObject.SetActive(completed);
            float preferredHeight = transform.Find("TaskDescription").GetComponent<TMPro.TextMeshProUGUI>().preferredHeight;
            if (completed)
            {
                rewardArea.GetComponent<RewardAreaHandler>().SetRewards(this.task!.GetRewards());
                preferredHeight += rewardArea.GetComponent<LayoutElement>().preferredHeight;
            }
            transform.GetComponent<LayoutElement>().preferredHeight = preferredHeight;
            needUpdate = false;
        }
    }

    public void SetTask(GameTask? task)
    {
        this.task = task;
        needUpdate = true;
    }

    protected virtual void OnRectTransformDimensionsChange()
    {
        needUpdate = true;
    }

    private async Task UpdateDescription()
    {
        TMPro.TextMeshProUGUI taskDescription = transform.Find("TaskDescription").GetComponent<TMPro.TextMeshProUGUI>();
        taskDescription.text = "";
        if (this.task != null)
        {
            string description = await this.task.GetDescription();
            int? progress = this.task.GetProgress();
            int? total = this.task.GetTotal();
            if ((progress != null) && (total != null))
            {
                description += $": {progress}/{total}";
            }

            if (task.IsCompleted())
            {
                description += "  <sprite name=\"Check\">";
            }
            taskDescription.text = description;
        }
    }   
}
