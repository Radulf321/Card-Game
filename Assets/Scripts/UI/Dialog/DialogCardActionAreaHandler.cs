using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

#nullable enable

public class DialogCardActionAreaHandler : GenericCardsContainerHandler<DialogOption, DialogOptionCardHandler>
{
    private List<DialogOption>? dialogOptions;
    private TaskCompletionSource<DialogOption>? selectedTask;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetDialogOptions(List<DialogOption> dialogOptions)
    {
        this.dialogOptions = dialogOptions;
        updateView();
    }

    public void SetSelectedTask(TaskCompletionSource<DialogOption> selectedTask)
    {
        this.selectedTask = selectedTask;
    }

    protected override List<DialogOption> GetCardData()
    {
        return this.dialogOptions!;
    }

    protected override DialogOption GetHandlerData(DialogOptionCardHandler cardHandler)
    {
        return cardHandler.GetOption()!;
    }

    protected override void SetHandlerData(DialogOptionCardHandler cardHandler, DialogOption cardData)
    {
        cardHandler.SetOption(cardData);
        cardHandler.SetOnClick(() =>
        {
            this.selectedTask?.SetResult(cardData);
        });
    }

    protected override DialogOptionCardHandler GetHandler(Transform transform)
    {
        return transform.GetComponent<DialogOptionCardHandler>();
    }
}
