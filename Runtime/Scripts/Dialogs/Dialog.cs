using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

#nullable enable
abstract public class Dialog
{
    public static Dialog? CurrentDialog;

    public static Dialog? FromJson(JArray? dialogData, Dialog? nextDialog = null)
    {
        if (dialogData == null || dialogData.Count == 0)
        {
            throw new System.Exception("Dialog JSON is null or empty.");
        }
        if (dialogData.Count == 1)
        {
            JToken firstToken = dialogData.First!;
            return FromJson(firstToken, nextDialog);
        }
        dialogData = new JArray(dialogData);
        JToken singleDialogData = dialogData[dialogData.Count - 1];
        dialogData.RemoveAt(dialogData.Count - 1);
        return FromJson(dialogData, FromJson(singleDialogData, nextDialog));
    }

    public static Dialog FromJson(JObject dialogData, Dialog? nextDialog = null)
    {
        string type = dialogData["type"]?.ToString() ?? "Undefined";
        switch (type)
        {
            case "text":
                return new DialogText(dialogData, nextDialog);
            case "image":
                return new DialogImage(dialogData, nextDialog);
            case "reward":
                return new DialogReward(dialogData, nextDialog);
            case "select":
                return new DialogSelect(dialogData, nextDialog);
            case "flag":
                return new DialogFlag(dialogData, nextDialog);
            case "condition":
                return new DialogCondition(dialogData, nextDialog);
            case "random":
                return new DialogRandom(dialogData, nextDialog);
            case "jump":
                return new DialogJump(dialogData, nextDialog);
            default:
                throw new System.Exception("Dialog type not recognized: " + type);
        }
    }

    public static Dialog? FromJson(JToken? json, Dialog? nextDialog = null)
    {
        switch (json?.Type)
        {
            case JTokenType.Array:
                return Dialog.FromJson(json as JArray, nextDialog);

            case JTokenType.Object:
                return Dialog.FromJson((json as JObject)!, nextDialog);

            default:
                return null;
        }
    }

    public static Dialog? GetDialogByID(string id)
    {
        if (Dialog.CurrentDialog == null) {
            return null;
        }
        Dialog? checkDialogs(List<Dialog> dialogs)
        {
            foreach (Dialog dialog in dialogs)
            {
                if (dialog.GetID() == id)
                {
                    return dialog;
                }
                Dialog? recursiveResult = checkDialogs(dialog.GetFollowingDialogs());
                if (recursiveResult != null)
                {
                    return recursiveResult;
                }
            }
            return null;
        }

        return checkDialogs(new List<Dialog>() {Dialog.CurrentDialog});
    }

    protected static string? GetIDFromJson(JObject json)
    {
        return json["id"]?.ToString();
    }

    private Dialog? nextDialog;
    private string? id;

    protected Dialog(Dialog? nextDialog = null, string? id = null)
    {
        this.nextDialog = nextDialog;
        this.id = id;
    }

    public string? GetID()
    {
        return this.id;
    }

    virtual public List<Dialog> GetFollowingDialogs()
    {

        List<Dialog> result = new List<Dialog>();
        if (nextDialog != null)
        {
            result.Add(nextDialog);
        }
        return result;
    }

    virtual public Task ShowDialog()
    {
        if (nextDialog != null)
        {
            return nextDialog.ShowDialog();
        }
        return Task.CompletedTask;
    }
}