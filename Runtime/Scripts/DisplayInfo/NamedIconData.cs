#nullable enable

using System;
using Newtonsoft.Json.Linq;
using UnityEngine;

public class NamedIconData {
    private string id;
    protected string? name;
    protected string? iconName;

    public NamedIconData(JObject experienceTypeData) {
        this.id = experienceTypeData["id"]?.ToString() ?? "Undefined";
        this.name = LocalizationHelper.GetLocalizedString(experienceTypeData["name"] as JObject);
        this.iconName = experienceTypeData["icon"]?.ToString();
    }

    public virtual string GetName() {
        if (name == null) {
            throw new Exception($"No name found for id {id}");
        }
        return name;
    }

    public virtual string GetInlineIcon() {
        if (iconName == null) {
            return this.GetName();
        }
        return "<sprite name=\"" + iconName + "\">";
    }

    public virtual Sprite? GetIcon() {
        if (iconName == null) {
            return null;
        }
        return Game.Instance?.GetIcon(this.iconName);
    }
}