using System;
using Newtonsoft.Json.Linq;
using Unity.VisualScripting;

#nullable enable
abstract public class Dialog {
    public static Dialog FromJson(JArray? dialogData, Action? onFinish = null, Func<string?, Action>? actionGenerator = null) {
        if (dialogData == null || dialogData.Count == 0) {
            throw new System.Exception("Dialog JSON is null or empty.");
        }
        if (dialogData.Count == 1) {
            return FromJson(dialogData[0] as JObject ?? new JObject(), onFinish, actionGenerator);
        }
        dialogData = new JArray(dialogData);
        JObject singleDialogData = dialogData[dialogData.Count - 1] as JObject ?? new JObject();
        dialogData.RemoveAt(dialogData.Count - 1);
        return FromJson(dialogData, () => {
            FromJson(singleDialogData, onFinish, actionGenerator).ShowDialog();
        }, actionGenerator);
    }

    public static Dialog FromJson(JObject dialogData, Action? onFinish = null, Func<string?, Action>? actionGenerator = null) {
        string type = dialogData["type"]?.ToString() ?? "Undefined";
        switch (type) {
            case "text":
                return new DialogText(dialogData, onFinish ?? DialogHandler.dialogFinish ?? (() => { }));
            case "image":
                return new DialogImage(dialogData, onFinish ?? DialogHandler.dialogFinish ?? (() => { }));
            case "reward":
                return new DialogReward(dialogData, onFinish ?? DialogHandler.dialogFinish ?? (() => { }));
            case "select":
                return new DialogSelect(dialogData, actionGenerator ?? (id => () => { DialogHandler.dialogFinish?.Invoke(); }));
            default:
                throw new System.Exception("Dialog type not recognized: " + type);
        }
    }
    abstract public void ShowDialog();
}