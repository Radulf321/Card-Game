using System;
using Newtonsoft.Json.Linq;

#nullable enable
public abstract class ActionCharacter {
    public static string CurrentTargetKey = "CurrentTarget";

    private string id;
    private string? actionTitle;
    private string actionDescription;
    private string actionImagePath;

    public ActionCharacter(string id) {
        this.id = id;
        this.actionTitle = null;
        this.actionDescription = "Not initialized";
        this.actionImagePath = "Placeholder";
    }

    public ActionCharacter(JObject jsonObject)
    {
        this.id = jsonObject["id"]!.ToString();
        JObject? action = jsonObject["action"] as JObject;
        if (action != null)
        {
            this.actionTitle = LocalizationHelper.GetLocalizedString(action!["title"] as JObject);
            this.actionDescription = LocalizationHelper.GetLocalizedString(action!["description"] as JObject)!;
            this.actionImagePath = action!["image"]!.ToString();
        }
        else
        {
            this.actionTitle = null;
            this.actionDescription = "Not initialized";
            this.actionImagePath = "Placeholder";
        }
    }

    public DialogOption? GetDialogOption() {
        if (this.actionTitle == null)
        {
            return null;
        }
        return new DialogOption(
            title: this.actionTitle,
            dialog: new DialogFlag<string>(
                value: GetID(),
                key: ActionCharacter.CurrentTargetKey,
                validity: FlagValidity.Dialog
            ),
            description: this.actionDescription,
            imagePath: this.actionImagePath
        );
    }

    public string GetID()
    {
        return this.id;
    }

    abstract public void ExecuteAction();
}