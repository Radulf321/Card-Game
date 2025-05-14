using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

#nullable enable
public class DialogText : Dialog
{
    private string text;
    private string? speaker;
    private Action onFinish;

    public DialogText(string text, Action onFinish, string? speaker = null)
    {
        this.text = text;
        this.speaker = speaker;
        this.onFinish = onFinish;
    }

    public DialogText(string text, Dialog nextDialog, string? speaker = null) : this(text, nextDialog.ShowDialog, speaker) { }

    public DialogText(List<string> texts, Action onFinish)
    {
        if (texts.Count == 0)
        {
            throw new ArgumentException("List of texts cannot be empty.");
        }
        
        this.text = texts[0];
        if (texts.Count == 1)
        {
            this.onFinish = onFinish;
        }
        else {
            this.onFinish = () =>
            {
                Dialog nextDialog = new DialogText(texts.GetRange(1, texts.Count - 1), onFinish);
                nextDialog.ShowDialog();
            };
        }
    }

    public DialogText(List<string> texts, Dialog nextDialog) : this(texts, nextDialog.ShowDialog) { }

    public DialogText(JObject dialogData, Action onFinish) : this(LocalizationHelper.GetLocalizedString(dialogData["text"] as JObject), onFinish, dialogData["speaker"]?.ToString()) { }

    public string GetText()
    {
        return text;
    }

    public string? GetSpeaker()
    {
        return speaker;
    }

    public Action GetOnFinish()
    {
        return onFinish;
    }

    public override void ShowDialog()
    {
        UnityEngine.Object.FindAnyObjectByType<DialogHandler>().ShowText(this);
    }
}