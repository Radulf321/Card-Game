using Newtonsoft.Json.Linq;

#nullable enable
public abstract class ActionCharacter {
    private string? actionTitle;
    private string actionDescription;
    private string actionImagePath;

    public ActionCharacter() {
        this.actionTitle = null;
        this.actionDescription = "Not initialized";
        this.actionImagePath = "Placeholder";
    }

    public ActionCharacter(JObject jsonObject)
    {
        JObject? action = jsonObject["action"] as JObject;
        if (action != null)
        {
            this.actionTitle = LocalizationHelper.GetLocalizedString(action!["title"] as JObject);
            this.actionDescription = LocalizationHelper.GetLocalizedString(action!["description"] as JObject);
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
            this.actionTitle,
            () =>
            {
                ExecuteAction();
            },
            this.actionDescription,
            this.actionImagePath
        );
    }

    abstract public void ExecuteAction();
}