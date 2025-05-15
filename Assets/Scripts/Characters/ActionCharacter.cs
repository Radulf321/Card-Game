using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine.SceneManagement;

#nullable enable
public abstract class ActionCharacter {
    private string actionTitle;
    private string actionDescription;
    private string actionImagePath;

    public ActionCharacter() {
        this.actionTitle = "Not initialized";
        this.actionDescription = "Not initialized";
        this.actionImagePath = "Placeholder";
    }

    public ActionCharacter(JObject jsonObject) {
        JObject? action = jsonObject["action"] as JObject;
        this.actionTitle = LocalizationHelper.GetLocalizedString(action?["title"] as JObject) ?? "Unknown Title";
        this.actionDescription = LocalizationHelper.GetLocalizedString(action?["description"] as JObject) ?? "Undefined Description";
        this.actionImagePath = action?["image"]?.ToString() ?? "Placeholder";
    }

    public DialogOption GetDialogOption() {
        return new DialogOption(
            this.actionTitle,
            () => {
                ExecuteAction();
            },
            this.actionDescription,
            this.actionImagePath
        );
    }

    abstract protected void ExecuteAction();
}