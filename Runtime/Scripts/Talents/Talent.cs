using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

#nullable enable
public class Talent
{

    private string id;
    private string title;
    private Dictionary<string, int> cost;
    private string description;
    private string imagePath;
    private List<string> prerequisites;
    private Dialog dialog;
    private bool purchased = false;
    private CombatTarget owner;

    public Talent(string title, Dictionary<string, int> cost, string description, string imagePath, List<Reward> rewards, List<Talent> prerequisites, Dialog dialog, CombatTarget owner)
    {
        this.id = title;
        this.title = title;
        this.cost = cost;
        this.description = description;
        this.imagePath = imagePath;
        this.prerequisites = prerequisites.ConvertAll(t => t.GetTitle());
        this.dialog = dialog;
        this.owner = owner;
    }

    public Talent(JObject talentData, CombatTarget owner)
    {
        this.id = talentData["id"]?.ToString() ?? "Undefined";
        this.title = LocalizationHelper.GetLocalizedString(talentData["title"] as JObject)!;
        Dictionary<string, int> cost = new Dictionary<string, int>();
        JObject costObject = talentData["cost"] as JObject ?? new JObject();
        foreach (JProperty property in costObject.Properties())
        {
            string type = property.Name;
            int value = property.Value.ToObject<int>();
            cost[type] = value;
        }
        this.cost = cost;
        this.description = LocalizationHelper.GetLocalizedString(talentData["description"] as JObject)!;
        this.imagePath = talentData["image"]?.ToString() ?? "Placeholder";
        this.prerequisites = talentData["prerequisites"]?.ToObject<List<string>>() ?? new List<string>();
        this.dialog = Dialog.FromJson(talentData["dialog"] as JArray ?? new JArray());
        this.owner = owner;
    }

    public string GetID()
    {
        return this.id;
    }

    public string GetTitle()
    {
        return this.title;
    }

    public Dictionary<string, int> GetCost()
    {
        return this.cost;
    }

    public async Task<string> GetDescription()
    {
        List<Reward> rewards = this.GetRewards();
        if (rewards.Count == 0)
        {
            return this.description;
        }
        else
        {
            string rewardName;
            if (LocalizationHelper.GetLocalization() == "de")
            {
                rewardName = "Belohnung";
            }
            else
            {
                rewardName = "Reward";
            }
            HashSet<string> rewardStrings = new HashSet<string>();
            foreach (Reward reward in rewards)
            {
                rewardStrings.Add(await reward.ToNiceString());
            }
            return this.description + "\n\n" + rewardName + ": " + string.Join(",", rewardStrings);
        }
    }

    public string GetInfoDescription()
    {
        return this.description;
    }

    public string GetImagePath()
    {
        return Game.Instance.GetResourcePath() + "/Graphics/Talents/" + this.imagePath;
    }

    public List<string> GetPrerequisites()
    {
        return this.prerequisites;
    }

    public List<Reward> GetRewards()
    {
        return ExtractRewardsFromDialog(this.dialog);
    }

    public Dialog GetDialog()
    {
        return this.dialog;
    }

    public bool IsPurchased()
    {
        return this.purchased;
    }

    public async Task Purchase(Action? afterDialog = null)
    {
        if (this.purchased)
        {
            throw new System.Exception("Talent already purchased.");
        }

        foreach (string type in GetCost().Keys)
        {
            if (this.owner.GetExperience(type) < GetCost()[type])
            {
                throw new System.Exception("Not enough experience to purchase talent.");
            }
        }

        foreach (string type in GetCost().Keys)
        {
            owner.IncreaseExperience(type, -GetCost()[type]);
        }

        this.purchased = true;
        await DialogHandler.Instance!.StartDialog(GetDialog(), onFinish: afterDialog ?? (() =>
        {
            Game.Instance!.EndRound();
        }));
    }

    private List<Reward> ExtractRewardsFromDialog(Dialog dialog)
    {
        List<Reward> rewards = new List<Reward>();
        if (dialog is DialogReward rewardDialog)
        {
            rewards.AddRange(rewardDialog.GetRewards());
        }

        foreach (Dialog followingDialog in dialog.GetFollowingDialogs())
        {
            rewards.AddRange(ExtractRewardsFromDialog(followingDialog));
        }

        return rewards;
    }
}