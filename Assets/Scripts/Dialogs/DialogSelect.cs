using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

#nullable enable
public enum SelectType {
    Buttons,
    Cards,
}

public class DialogSelect : Dialog {
    private string title;
    private List<DialogOption> options;
    private SelectType selectType;
    private bool showUI;


    public DialogSelect(string title, List<DialogOption> options, SelectType selectType = SelectType.Buttons, bool showUI = false) {
        this.showUI = showUI;
        this.title = title;
        this.options = options;
        this.selectType = selectType;
    }

    public DialogSelect(JObject dialogData, Func<string?, Action> actionGenerator) {
        this.title = LocalizationHelper.GetLocalizedString(dialogData["title"] as JObject);
        this.showUI = dialogData["showUI"]?.ToObject<bool>() ?? false;
        this.selectType = (SelectType)Enum.Parse(typeof(SelectType), dialogData["selectType"]?.ToString() ?? "Buttons");
        List<DialogOption> options = new List<DialogOption>();
        foreach (JObject optionData in dialogData["options"] ?? new JArray()) {
            options.Add(new DialogOption(optionData, actionGenerator(optionData["id"]?.ToString())));
        }
        this.options = options;
    }

    public override void ShowDialog() {
        UnityEngine.Object.FindAnyObjectByType<DialogHandler>().ShowSelect(this);
    }

    public string GetTitle() {
        return this.title;
    }

    public List<DialogOption> GetOptions() {
        return this.options;
    }

    public SelectType GetSelectType() {
        return this.selectType;
    }

    public bool IsShowUI() {
        return this.showUI;
    }
}