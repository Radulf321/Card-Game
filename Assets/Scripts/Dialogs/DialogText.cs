using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

#nullable enable
public class DialogText : Dialog
{
    private string text;
    private string? speaker;

    public DialogText(string text, string? speaker = null, Dialog? nextDialog = null, string? id = null) : base(nextDialog: nextDialog, id: id)
    {
        this.text = text;
        this.speaker = speaker;
    }

    public DialogText(List<string> texts, Dialog? nextDialog = null, string? id = null) :
        this(
            text: texts[0],
            nextDialog: texts.Count > 1 ?
                new DialogText(texts.GetRange(1, texts.Count - 1), nextDialog, id) :
                nextDialog,
            id: id
        )
    {
    }

    public DialogText(JObject dialogData, Dialog? nextDialog = null) : this(
        text: LocalizationHelper.GetLocalizedString(dialogData["text"] as JObject)!,
        speaker: dialogData["speaker"]?.ToString(),
        nextDialog: nextDialog,
        id: Dialog.GetIDFromJson(dialogData)
    )
    { }

    public string GetText()
    {
        return text;
    }

    public string? GetSpeaker()
    {
        return speaker;
    }

    public override async Task ShowDialog()
    {
        await DialogHandler.Instance!.ShowText(this);
        await base.ShowDialog();
    }
}