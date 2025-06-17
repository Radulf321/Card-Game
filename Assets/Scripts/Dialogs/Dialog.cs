using System;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

#nullable enable
abstract public class Dialog {
    public static Dialog FromJson(JArray? dialogData, Dialog? nextDialog = null) {
        if (dialogData == null || dialogData.Count == 0)
        {
            UnityEngine.Debug.Log("Throwing exception");
            throw new System.Exception("Dialog JSON is null or empty.");
        }
        if (dialogData.Count == 1) {
            return FromJson(dialogData[0] as JObject ?? new JObject(), nextDialog);
        }
        dialogData = new JArray(dialogData);
        JObject singleDialogData = dialogData[dialogData.Count - 1] as JObject ?? new JObject();
        dialogData.RemoveAt(dialogData.Count - 1);
        return FromJson(dialogData, FromJson(singleDialogData, nextDialog));
    }

    public static Dialog FromJson(JObject dialogData, Dialog? nextDialog = null) {
        string type = dialogData["type"]?.ToString() ?? "Undefined";
        switch (type) {
            case "text":
                return new DialogText(dialogData, nextDialog);
            case "image":
                return new DialogImage(dialogData, nextDialog);
            case "reward":
                return new DialogReward(dialogData, nextDialog);
            case "select":
                return new DialogSelect(dialogData, nextDialog);
            default:
                UnityEngine.Debug.Log("Dialog type not recognized: " + type);
                throw new System.Exception("Dialog type not recognized: " + type);
        }
    }

    public static Dialog? FromJson(JToken? json, Dialog? nextDialog = null)
    {
        switch (json?.Type)
        {
            case JTokenType.Array:
                return Dialog.FromJson(json as JArray);

            case JTokenType.Object:
                return Dialog.FromJson((json as JObject)!);

            default:
                return null;
        }
    }

    protected Dialog? nextDialog;

    protected Dialog(Dialog? nextDialog = null) {
        this.nextDialog = nextDialog;
    }

    virtual public Task ShowDialog() {
        if (nextDialog != null) {
            return nextDialog.ShowDialog();
        }
        return Task.CompletedTask;
    }
}