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

        Talent introduction = new Talent(
            "Introduction",
            new Dictionary<string, int>() {},
            "Your first meeting with Jane",
            "Placeholder",
            new List<Reward>() {},
            new List<Talent>(),
            new DialogText(new List<string>() {
                "As you walk into the bar, you see an attractive redhead sitting at the bar. You decide to approach her.",
                "You: Hi, I'm Bob. What brings you here?",
                "Jane: I'm just here to relax and have a drink. What about you?",
                "You: Same here. Mind if I join you?",
                "Jane: Not at all. I'm Jane, by the way.",
            },
            () => { DialogHandler.dialogFinish?.Invoke(); }),
            this
        );

        Talent romanticKiss = new Talent(
            "Romantic Kiss",
            new Dictionary<string, int>() {{ "love", 1 }},
            "After a date, you lean in for a kiss",
            "Placeholder",
            new List<Reward>() {
                new CardReward(CardLibrary.getCard(CardType.Flirt)),
            },
            new List<Talent>() { introduction },
            new DialogText(new List<string>() {
                "Jane: Wow, it's already getting late. I should probably head home.",
                "Jane: I head a great time with you, we should definitely repeat this.",
                "You: Wow. So late already? Time flies when you're having fun.",
                "You: I had a great time too. I hope I'll see you again soon.",
                "As you get up to leave, you lean in and give Jane a quick kiss on the lips.",
                "Jane seems surprised but she holds you in place and deepens the kiss.",
                "She slips her tongue into your mouth and you can feel her warm breath on your face.",
                "Jane: Let's do this again sometime.",
            },
            () => { DialogHandler.dialogFinish?.Invoke(); }),
            this
        );

        Talent cunnilingus = new Talent(
            "Cunnilingus",
            new Dictionary<string, int>() {{ "love", 1 }},
            "A wonderful date gets even better as you walk Jane home and she invites you to lick her pussy.",
            "Placeholder",
            new List<Reward>() {
                new CardReward(CardLibrary.getCard(CardType.Flirt)),
            },
            new List<Talent>() { romanticKiss },
            new DialogText(new List<string>() {
                "After the date, you walk Jane home.",
                "Jane: So, where was a guy like you hiding all this time?",
                "You: Just waiting for the perfect moment to meet someone like you.",
                "Jane: Aww, I bet you say that to all the women you meet.",
                "You: Only the special ones.",
                "As you keep walking, you arrive at Jane's place.",
                "Jane: So, why don't you come in? I could use some company.",
                "You: Sure, I'd love to.",
                "You walk into her apartment and she closes the door behind you.",
                "Right away, she pushes you against the wall and kisses you passionately.",
                "You can feel her warm body against yours and her hands exploring your body.",
                "As you take a short breath, you pull her top off.",
                "She smiles and continues by unclasping her bra as her breasts pop out.",
                "You: Wow, they're beautiful.",
                "Jane: Thanks, but I think you should be the one to show me how much you like them.",
                "You grab her breasts and start fondling them. Jane moans softly as you do.",
                "You: I think it's time to take this to the next level.",
                "TODO: Write the rest of the scene"                
            },
            () => { DialogHandler.dialogFinish?.Invoke(); }),
            this
        );

        this.introductionTalent = introduction;
        this.talents = new List<Talent>() { introduction, romanticKiss, cunnilingus };
        this.name = "Jane";
        this.actionTitle = "Date with Jane";
        this.actionDescription = "Get to know Jane and increase her experience levels";
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
            DialogHandler.StartDialog(
                Dialog.FromJson(this.loseDialogData, onFinish: () => {
                    Game.Instance.EndTurn();
                })
            );
            /*DialogHandler.StartDialog(
                new DialogText(
                    new List<string>() {
                        "Jane: Hey, it seems you're not really into this. Maybe we should try this again some other time.",
                        "Jane leaves you alone. You ponder what went wrong (-1 remaining turn)."
                    },
                    () => {
                        Game.Instance.EndTurn();
                    }
                )
            );*/
        }
        else {
            DialogHandler.dialogFinish = () => {
                Debug.Log("Dialog finish called");
            };
            DialogHandler.StartDialog(
                Dialog.FromJson(this.winDialogData, actionGenerator: (id) => {
                    return () => {
                        Debug.Log("Combat finished, id: " + id);
                        if (id != null) {
                            this.IncreaseExperience(id, 1);
                        }
                        this.IncreaseLevel();
                        SceneManager.LoadScene("TalentTreeScene");
                    };
                })
            );
            /*
            DialogHandler.StartDialog(
                new DialogText(
                    "Jane: I really enjoy spending time with you.",
                    new DialogSelect(
                        "Jane: I really enjoy spending time with you.",
                        new List<DialogOption>() {
                            new DialogOption("So did I, you are special (+ 1 Love)", () => {
                                this.IncreaseExperience("love", 1);
                                SceneManager.LoadScene("TalentTreeScene");
                            }),
                        }
                    )
                )
            );*/
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