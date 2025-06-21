#nullable enable
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using UnityEngine.Localization.Settings;

public class DialogOption
{
    private string? title;
    private string? imagePath;
    private string? description;
    private Dialog? dialog;
    private Card? card;
    private Dictionary<string, int>? cost;

    public DialogOption(string? title = null, Dialog? dialog = null, string? description = null, string? imagePath = null, string? id = null, Dictionary<string, int>? cost = null, Card? card = null)
    {
        this.title = title;
        this.description = description;
        this.dialog = dialog;
        this.imagePath = imagePath;
        this.cost = cost;
        this.card = card;
    }

    public DialogOption(JObject optionData)
    {
        this.title = LocalizationHelper.GetLocalizedString(optionData["title"] as JObject);
        this.description = (optionData["description"] != null) ? LocalizationHelper.GetLocalizedString(optionData["description"] as JObject) : null;
        this.dialog = Dialog.FromJson(optionData["dialog"]);
        this.imagePath = optionData?["image"]?.ToString();
        this.cost = JSONHelper.ObjectToDictionary<int>(optionData?["cost"] as JObject);
        this.card = (optionData?["card"] != null) ? Game.Instance.GetCard(optionData["card"]!.ToString()) : null;
    }

    public string? GetDescription()
    {
        return this.description;
    }

    public string? GetTitle()
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

    public Card? GetCard()
    {
        return this.card;
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