using System;
using System.Threading.Tasks;
using UnityEngine;

#nullable enable

public class DialogOptionHandler : MonoBehaviour
{
    private Action? onClickAction;
    private TaskCompletionSource<DialogOption>? selectedTask;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetDialogOption(DialogOption option)
    {
        String title = option.GetTitle() ?? option.GetCard()?.GetName() ?? "This should never be visible";
        transform.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = title;
        this.onClickAction = () =>
        {
            option.Select(() =>
            {
                this.selectedTask?.SetResult(option);
            });
        };

        option.GetCostText().ContinueWith((Task<string?> task) => {
            string? costText = task.Result;
            if (costText != null) {
                transform.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = title + " (" + costText + ")";
            }
        }, TaskScheduler.FromCurrentSynchronizationContext());
    }

    public void SetSelectedTask(TaskCompletionSource<DialogOption> selectedTask)
    {
        this.selectedTask = selectedTask;
    }

    public void OnClick()
    {
        this.onClickAction?.Invoke();
    }
}
