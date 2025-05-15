#nullable enable
using System;
using Newtonsoft.Json.Linq;
using UnityEngine;

public class DialogOption {
    private string title;
    private string? imagePath;
    private string? description;
    private Action action;

    public DialogOption(string title, Action action, string? description = null, string? imagePath = null, string? id = null) {
        this.title = title;
        this.description = description;
        this.action = action;
        this.imagePath = imagePath;
    }

    public DialogOption(JObject dialogData, Action action) {
        this.title = LocalizationHelper.GetLocalizedString(dialogData["title"] as JObject);
        this.description = (dialogData["description"] != null) ? LocalizationHelper.GetLocalizedString(dialogData["description"] as JObject) : null;
        this.action = action;
        this.imagePath = dialogData["image"]?.ToString();
    }

    public string? GetDescription() {
        return this.description;
    }

    public Action GetAction() {
        return this.action;
    }

    public string GetTitle() {
        return this.title;
    }

    public string? GetImagePath() {
        if (this.imagePath == null) {
            return null;
        }
        return Game.Instance.GetResourcePath() +  "/Graphics/Dialog/" + this.imagePath;
    }
}