using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

#nullable enable

public class DialogCardActionAreaHandler : CardsContainerHandler<DialogOption>
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

    protected override DialogOption GetHandlerData(CardHandler cardHandler)
    {
        string title = cardHandler.GetTitle();
        return dialogOptions!.Find(option => option.GetTitle() == title);
    }

    protected override void SetHandlerData(CardHandler cardHandler, DialogOption cardData)
    {
        cardHandler.SetTitle(cardData.GetTitle());
        cardHandler.SetDescription(cardData.GetDescription());
        cardHandler.SetSprite(Resources.Load<Sprite>(cardData.GetImagePath()));
        cardHandler.SetCostSprite(null);
        cardHandler.SetOnClickAction(() =>
        {
            this.selectedTask?.SetResult(cardData);
        });
    }
}
