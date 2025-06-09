#nullable enable

using Newtonsoft.Json.Linq;
using UnityEngine;

public class NamedIconData {
    private string id;
    private string name;
    private string? iconName;

    public NamedIconData(JObject experienceTypeData) {
        this.id = experienceTypeData["id"]?.ToString() ?? "Undefined";
        this.name = LocalizationHelper.GetLocalizedString(experienceTypeData["name"] as JObject);
        this.iconName = experienceTypeData["icon"]?.ToString();
    }

    public string GetName() {
        return name;
    }

    public string GetInlineIcon() {
        if (iconName == null) {
            return this.GetName();
        }
        return "<sprite name=\"" + iconName + "\">";
    }

    public Sprite? GetIcon() {
        if (iconName == null) {
            return null;
        }
        return Game.Instance?.GetIcon(this.iconName);
    }
}