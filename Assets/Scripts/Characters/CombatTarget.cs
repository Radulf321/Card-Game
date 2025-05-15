using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

#nullable enable
public class CombatTarget {
    private int level;
    private Dictionary<string, int> experience;

    private List<Talent> talents;
    private Talent? introductionTalent;
    private string name;
    private string actionTitle;
    private string actionDescription;
    private string actionImagePath;

    private string combatBackgroundPath;
    private string talentBackgroundPath;
    private List<RequirementFactory> requirementFactories;
    private JArray winDialogData;
    private JArray loseDialogData;

    public CombatTarget() {
        this.level = 1;
        this.experience = new Dictionary<string, int>();
        this.introductionTalent = null;
        this.talents = new List<Talent>();
        this.name = "Not initialized";
        this.actionTitle = "Not initialized";
        this.actionDescription = "Not initialized";
        this.actionImagePath = "Placeholder";
        this.combatBackgroundPath = "Placeholder";
        this.requirementFactories = new List<RequirementFactory>();
        this.winDialogData = new JArray();
        this.loseDialogData = new JArray();
        this.talentBackgroundPath = "Placeholder";
    }

    public CombatTarget(JObject jsonObject) {
        this.level = 1;
        this.experience = new Dictionary<string, int>();

        this.name = LocalizationHelper.GetLocalizedString(jsonObject["name"] as JObject);
        JObject? action = jsonObject["action"] as JObject;
        this.actionTitle = LocalizationHelper.GetLocalizedString(action?["title"] as JObject) ?? "Unknown Title";
        this.actionDescription = LocalizationHelper.GetLocalizedString(action?["description"] as JObject) ?? "Undefined Description";
        this.actionImagePath = action?["image"]?.ToString() ?? "Placeholder";

        List<Talent> talents = new List<Talent>();
        foreach (JObject talentData in jsonObject["talents"] ?? new JArray()) {
            Talent talent = new Talent(talentData, this);
            if (talentData["introduction"]?.ToObject<bool>() == true) {
                this.introductionTalent = talent;
            }
            talents.Add(talent);
        }
        this.talents = talents;

        List<RequirementFactory> requirementFactories = new List<RequirementFactory>();
        foreach (JObject requirementData in jsonObject["combat"]?["requirements"] ?? new JArray()) {
            requirementFactories.Add(RequirementFactory.FromJson(requirementData, this));
        }
        this.requirementFactories = requirementFactories;
        this.combatBackgroundPath = jsonObject["combat"]?["background"]?.ToString() ?? "Placeholder";
        this.talentBackgroundPath = jsonObject["talentBackground"]?.ToString() ?? "Placeholder";
        this.winDialogData = jsonObject["combat"]?["win"]?["dialog"] as JArray ?? new JArray();
        this.loseDialogData = jsonObject["combat"]?["lose"]?["dialog"] as JArray ?? new JArray();
    }

    public List<Turn> GenerateTurns() {
        List<Turn> turns = new List<Turn>();
        for (int i = 0; i <= this.level; i++) {
            turns.Add(new Turn());
            turns.Add(new Turn(GetRandomRequirements(i)));
        }

        return turns;
    }

    public int GetLevel() {
        return this.level;
    }

    public void IncreaseLevel() {
        this.level++;
    }

    public Dictionary<string, int> GetExperience() {
        return this.experience;
    }

    public int GetExperience(string type) {
        if (this.experience.ContainsKey(type)) {
            return this.experience[type];
        } else {
            return 0;
        }
    }

    public void IncreaseExperience(string type, int amount) {
        if (this.experience.ContainsKey(type)) {
            this.experience[type] += amount;
        } else {
            this.experience.Add(type, amount);
        }
    }

    public List<Talent> GetTalents() {
        return this.talents;
    }

    public Talent? GetTalent(string id) {
        foreach (Talent talent in this.talents) {
            if (talent.GetID() == id) {
                return talent;
            }
        }
        return null;
    }

    public void StartCombat() {
        Action start = () => {
            SceneManager.LoadScene("CombatScene");
        };
        // == false as introductionTalent could be null
        if (this.introductionTalent?.IsPurchased() == false) {
            DialogHandler.dialogFinish = start;
            this.introductionTalent?.Purchase();
        }
        else {
            start.Invoke();
        }
    }

    public void EndCombat(bool win) {
        if (!win) {
            Game.Instance.AddRemainingTurns(-1);
            DialogHandler.StartDialog(
                Dialog.FromJson(this.loseDialogData, onFinish: () => {
                    Game.Instance.EndTurn();
                })
            );
        }
        else {
            DialogHandler.StartDialog(
                Dialog.FromJson(this.winDialogData, actionGenerator: (id) => {
                    return () => {
                        if (id != null) {
                            this.IncreaseExperience(id, 1);
                        }
                        this.IncreaseLevel();
                        SceneManager.LoadScene("TalentTreeScene");
                    };
                })
            );
        }
    }

    private List<Requirement> GetRandomRequirements(int goalNumber) {
        // No Lust requirement for goal 0
        int numberOfRequirements = ((goalNumber + 1) / 2) + 1;
        List<int> requirementIndexes = new List<int>();
        List<Requirement> requirements = new List<Requirement>();
        for (int i = 0; i < numberOfRequirements; i++) {
            int random;
            do {
                random = UnityEngine.Random.Range(0, this.requirementFactories.Count);
            } while (requirementIndexes.Contains(random));
            requirementIndexes.Add(random);
            requirements.Add(this.requirementFactories[random].CreateRequirement(goalNumber * 2));
        }

        return requirements;
    }

    public DialogOption GetDialogOption() {
        return new DialogOption(
            this.actionTitle,
            () => {
                this.StartCombat();
            },
            this.actionDescription,
            this.actionImagePath
        );
    }

    public string GetCombatBackgroundPath() {
        return Game.Instance!.GetResourcePath() + "/Graphics/Backgrounds/" + this.combatBackgroundPath;
    }

    public string GetTalentBackgroundPath() {
        return Game.Instance!.GetResourcePath() + "/Graphics/Backgrounds/" + this.talentBackgroundPath;
    }
}