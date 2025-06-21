using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

#nullable enable
public enum SelectType
{
    Buttons,
    Cards,
}

public class DialogSelect : Dialog
{
    private string title;
    private List<DialogOptionFactory> optionFactories;
    private SelectType selectType;
    private bool showUI;

    public DialogSelect(string title, List<DialogOptionFactory> optionFactories, SelectType selectType = SelectType.Buttons, bool showUI = false, Dialog? nextDialog = null, string? id = null) : base(nextDialog: nextDialog, id: id)
    {
        this.showUI = showUI;
        this.title = title;
        this.optionFactories = optionFactories;
        this.selectType = selectType;
    }

    public DialogSelect(string title, List<DialogOption> options, SelectType selectType = SelectType.Buttons, bool showUI = false, Dialog? nextDialog = null, string? id = null)
    : this(
        title: title,
        optionFactories: new List<DialogOptionFactory>()
        {
            new ConstantDialogOptionFactory(options)
        },
        selectType: selectType,
        showUI: showUI,
        nextDialog: nextDialog,
        id: id
    )
    {
    }

    public DialogSelect(JObject dialogData, Dialog? nextDialog = null) : base(nextDialog, Dialog.GetIDFromJson(dialogData))
    {
        this.title = LocalizationHelper.GetLocalizedString(dialogData["title"] as JObject)!;
        this.showUI = dialogData["showUI"]?.ToObject<bool>() ?? false;
        this.selectType = (SelectType)Enum.Parse(typeof(SelectType), dialogData["selectType"]?.ToString() ?? "Buttons");
        List<DialogOptionFactory> options = new List<DialogOptionFactory>();
        foreach (JObject optionData in dialogData["options"] ?? new JArray())
        {
            options.Add(DialogOptionFactory.FromJSON(optionData));
        }
        this.optionFactories = options;
    }

    public override async Task ShowDialog()
    {
        DialogOption selectedOption = await DialogHandler.Instance!.ShowSelect(this);
        Dialog? optionDialog = selectedOption.GetDialog();
        if (optionDialog != null)
        {
            await optionDialog.ShowDialog();
        }
        await base.ShowDialog();
    }

    public string GetTitle()
    {
        return this.title;
    }

    public List<DialogOption> GetOptions(bool allOptions = false)
    {
        List<DialogOption> result = new List<DialogOption>();
        foreach (DialogOptionFactory factory in this.optionFactories)
        {
            result.AddRange(factory.GetOptions(allOptions));
        }
        return result;
    }

    public SelectType GetSelectType()
    {
        return this.selectType;
    }

    public bool IsShowUI()
    {
        return this.showUI;
    }

    public override List<Dialog> GetFollowingDialogs()
    {
        List<Dialog> result = base.GetFollowingDialogs();
        foreach (DialogOption option in GetOptions(true))
        {
            Dialog? dialog = option.GetDialog();
            if (dialog != null)
            {
                result.Add(dialog);
            }
        }
        return result;
    }
}