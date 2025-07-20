using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

#nullable enable
public class TaskOutcomeHandler : MonoBehaviour
{
    private GameTask? task;
    bool needUpdate = false;
    float previousParentHeight = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (needUpdate)
        {
            _ = UpdateDescription();
            Transform rewardArea = transform.Find("RewardDisplayArea");
            bool completed = task?.IsCompleted() ?? false;
            rewardArea.gameObject.SetActive(completed);
            if (completed)
            {
                float rewardHeight = transform.parent.GetComponent<RectTransform>().rect.height / 3;
                rewardArea.GetComponent<LayoutElement>().preferredHeight = rewardHeight;
                rewardArea.GetComponent<RewardAreaHandler>().SetRewards(this.task!.GetRewards());
            }
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
        float parentHeight = transform.parent.GetComponent<RectTransform>().rect.height;
        if (parentHeight != previousParentHeight)
        {
            previousParentHeight = parentHeight;
            needUpdate = true;
        }
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
