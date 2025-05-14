using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Unity.Properties;
using UnityEngine.SceneManagement;

#nullable enable
public class Talent {
    
    private string id;
    private string title;
    private Dictionary<string, int> cost;
    private string description;
    private string imagePath;
    private List<Reward> rewards;
    private List<string> prerequisites;
    private Dialog dialog;
    private bool purchased = false;
    private CombatTarget owner;

    public Talent(string title, Dictionary<string, int> cost, string description, string imagePath, List<Reward> rewards, List<Talent> prerequisites, Dialog dialog, CombatTarget owner) {
        this.id = title;
        this.title = title;
        this.cost = cost;
        this.description = description;
        this.imagePath = imagePath;
        this.rewards = rewards;
        this.prerequisites = prerequisites.ConvertAll(t => t.GetTitle());
        this.dialog = dialog;
        this.owner = owner;
    }

    public Talent(JObject talentData, CombatTarget owner) {
        this.id = talentData["id"]?.ToString() ?? "Undefined";
        this.title = LocalizationHelper.GetLocalizedString(talentData["title"] as JObject);
        Dictionary<string, int> cost = new Dictionary<string, int>();
        JObject costObject = talentData["cost"] as JObject ?? new JObject();
        foreach (JProperty property in costObject.Properties()) {
            string type = property.Name;
            int value = property.Value.ToObject<int>();
            cost[type] = value;
        }
        this.cost = cost;
        this.description = LocalizationHelper.GetLocalizedString(talentData["description"] as JObject);
        this.imagePath = talentData["imagePath"]?.ToString() ?? "Placeholder";
        List<Reward> rewards = new List<Reward>();
        foreach (JObject rewardData in talentData["rewards"] ?? new JArray()) {
            rewards.Add(Reward.FromJson(rewardData));
        }
        this.rewards = rewards;
        foreach (JProperty costProperty in (talentData["cost"] as JObject ?? new JObject()).Properties()) {
            string type = costProperty.Name;
            int value = costProperty.Value.ToObject<int>();
            this.cost[type] = value;
        }
        this.description = LocalizationHelper.GetLocalizedString(talentData["description"] as JObject);
        this.imagePath = talentData["image"]?.ToString() ?? "Placeholder";
        this.rewards = new List<Reward>();
        foreach (JObject rewardData in talentData["rewards"] ?? new JArray()) {
            this.rewards.Add(Reward.FromJson(rewardData));
        }
        this.prerequisites = talentData["prerequisites"]?.ToObject<List<string>>() ?? new List<string>();
        this.dialog = Dialog.FromJson(talentData["dialog"] as JArray ?? new JArray(), () => { DialogHandler.dialogFinish?.Invoke(); });
        this.owner = owner;
    }

    public string GetID() {
        return this.id;
    }

    public string GetTitle() {
        return this.title;
    }

    public Dictionary<string, int> GetCost() {
        return this.cost;
    }

    public string GetDescription() {
        if (this.rewards.Count == 0) {
            return this.description;
        }
        else {
            string rewardName;
            if (LocalizationHelper.GetLocalization() == "de") {
                rewardName = "Belohnung";
            } else {
                rewardName = "Reward";
            }
            return this.description + "\n\n" + rewardName + ": " + string.Join(",", this.rewards.ConvertAll(reward => reward.ToNiceString()));
        }
    }

    public string GetInfoDescription() {
        return this.description;
    }

    public string GetImagePath() {
        return Game.Instance.GetResourcePath() +  "/Graphics/Talents/" + this.imagePath;
    }

    public List<string> GetPrerequisites() {
        return this.prerequisites;
    }

    public List<Reward> GetRewards() {
        return this.rewards;
    }

    public Dialog GetDialog() {
        return this.dialog;
    }

    public bool IsPurchased() {
        return this.purchased;
    }

    public void Purchase() {
        if (this.purchased) {
            throw new System.Exception("Talent already purchased.");
        }

        foreach (string type in GetCost().Keys) {
            if (this.owner.GetExperience(type) < GetCost()[type]) {
                throw new System.Exception("Not enough experience to purchase talent.");
            }
        }
        
        foreach (string type in GetCost().Keys) {
            owner.IncreaseExperience(type, -GetCost()[type]);
        }

        foreach (Reward reward in GetRewards()) {
            reward.Collect();
        }

        DialogHandler.firstDialog = GetDialog();
        this.purchased = true;
        SceneManager.LoadScene("DialogScene");
    }
}