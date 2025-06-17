#nullable enable
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using UnityEngine.Localization.Settings;

public class DialogOption
{
    private string title;
    private string? imagePath;
    private string? description;
    private Dialog? dialog;
    private Dictionary<string, int>? cost;

    public DialogOption(string title, Dialog? dialog = null, string? description = null, string? imagePath = null, string? id = null, Dictionary<string, int>? cost = null)
    {
        this.title = title;
        this.description = description;
        this.dialog = dialog;
        this.imagePath = imagePath;
        this.cost = cost;
    }

    public DialogOption(JObject optionData)
    {
        this.title = LocalizationHelper.GetLocalizedString(optionData["title"] as JObject);
        this.description = (optionData["description"] != null) ? LocalizationHelper.GetLocalizedString(optionData["description"] as JObject) : null;
        switch (optionData?["dialog"]?.Type)
        {
            case JTokenType.Array:
                this.dialog = Dialog.FromJson(optionData["dialog"] as JArray);
                break;

            case JTokenType.Object:
                this.dialog = Dialog.FromJson((optionData["dialog"] as JObject)!);
                break;

            default:
                this.dialog = null;
                break;

        }
        this.imagePath = optionData?["image"]?.ToString();
        this.cost = JSONHelper.ObjectToDictionary<int>(optionData?["cost"] as JObject);
    }

    public string? GetDescription()
    {
        return this.description;
    }

    public string GetTitle()
    {
        return this.title;
    }

    public string? GetImagePath()
    {
        if (this.imagePath == null)
        {
            return null;
        }
        return Game.Instance.GetResourcePath() + "/Graphics/Dialog/" + this.imagePath;
    }

    public Dialog? GetDialog()
    {
        return this.dialog;
    }

    public Task<string?> GetCostText()
    {
        if (this.cost == null)
        {
            return Task.FromResult<string?>(null);
        }

        if (this.cost.Count == 0)
        {
            return AsyncHelper.HandleToTask(LocalizationSettings.StringDatabase.GetLocalizedStringAsync("UIStrings", "CostFree"));
        }

        List<string> results = new List<string>();
        foreach (KeyValuePair<string, int> entry in this.cost)
        {
            results.Add(entry.Value.ToString() + Game.Instance.GetCurrencyInlineIcon(entry.Key));
        }

        return Task.FromResult<string?>(string.Join(" ", results));
    }

    public void Select(Action? onSucess)
    {
        if (this.cost == null)
        {
            onSucess?.Invoke();
        }
        else
        {
            Player player = Game.Instance.GetPlayer();
            foreach (KeyValuePair<string, int> entry in this.cost)
            {
                if (player.GetCurrency(entry.Key) < entry.Value)
                {
                    // TODO: Show UI Feedback
                    return;
                }
            }
            foreach (KeyValuePair<string, int> entry in this.cost)
            {
                player.AddCurrency(entry.Key, -entry.Value);
            }
            onSucess?.Invoke();
        }
    }
}