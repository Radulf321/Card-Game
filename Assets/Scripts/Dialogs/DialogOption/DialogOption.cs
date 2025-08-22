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

    public async Task<string?> GetCostText()
    {
        if (this.cost == null)
        {
            return null;
        }

        if (this.cost.Count == 0)
        {
            return await AsyncHelper.HandleToTask(LocalizationSettings.StringDatabase.GetLocalizedStringAsync("UIStrings", "CostFree"));
        }

        List<string> results = new List<string>();
        foreach (KeyValuePair<string, int> entry in this.cost)
        {
            switch (entry.Key)
            {
                case "rounds":
                    results.Add(await AsyncHelper.HandleToTask(LocalizationSettings.StringDatabase.GetLocalizedStringAsync("UIStrings", "RoundsCost", arguments: new Dictionary<string, object> {
                        { "amount", entry.Value },
                    })));
                    break;

                default:
                    results.Add(entry.Value.ToString() + Game.Instance.GetCurrencyInlineIcon(entry.Key));
                    break;
            }
        }

        return string.Join(" ", results);
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
                switch (entry.Key)
                {
                    case "rounds":
                        if (Game.Instance.GetRemainingRounds() < entry.Value)
                        {
                            // TODO: Show UI Feedback
                            return;
                        }
                        break;

                    default:
                        if (player.GetCurrency(entry.Key) < entry.Value)
                        {
                            // TODO: Show UI Feedback
                            return;
                        }
                        break;
                }
            }
            foreach (KeyValuePair<string, int> entry in this.cost)
            {
                switch (entry.Key)
                {
                    case "rounds":
                        Game.Instance.AddRemainingRounds(-entry.Value);
                        break;

                    default:
                        player.AddCurrency(entry.Key, -entry.Value);
                        break;
                }
            }
            onSucess?.Invoke();
        }
    }
}