using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;

#nullable enable
class Game
{
    public static Game Instance { get; private set; } = new Game();

    private static readonly string tutorialFlag = "tutorialDone";

    private Player player;
    private int remainingRounds;
    private List<CombatTarget> combatTargets;
    private List<Location> locations;

    private Dictionary<string, string> goalNames;

    private Dictionary<string, ExperienceTypeData> experienceTypes;

    private Dictionary<string, Sprite> icons;

    private CardLibrary cardLibrary;

    private string resourcePath;

    private string selectActionBackground;
    private string checkIcon;
    private Dialog gameOverDialog;
    private CombatTarget? currentCombatTarget;
    private List<Action<TriggerMessage>> triggerMessageSubscribers = new List<Action<TriggerMessage>>();
    private Dictionary<string, bool> boolFlags;

    public Game()
    {
        this.player = new Player();
        this.remainingRounds = 0;
        this.combatTargets = new List<CombatTarget>();
        this.locations = new List<Location>();
        this.goalNames = new Dictionary<string, string>();
        this.experienceTypes = new Dictionary<string, ExperienceTypeData>();
        this.icons = new Dictionary<string, Sprite>();
        this.cardLibrary = new CardLibrary();
        this.resourcePath = "";
        this.selectActionBackground = "";
        this.checkIcon = "";
        this.gameOverDialog = new DialogText("Dummy", () => { });
        this.boolFlags = new Dictionary<string, bool>();
    }

    public Game(string ResourcePath)
    {
        Instance = this;
        this.resourcePath = ResourcePath;
        TMPro.TMP_Settings.defaultSpriteAsset = Resources.Load<TMPro.TMP_SpriteAsset>(ResourcePath + "/Graphics/Icons");
        JObject index = JObject.Parse(Resources.Load<TextAsset>(ResourcePath + "/Index").text);
        Dictionary<string, string> goalNames = new Dictionary<string, string>();
        foreach (JObject goal in index["goals"]!)
        {
            string id = goal["id"]!.ToString();
            string name = LocalizationHelper.GetLocalizedString(goal["name"] as JObject);
            goalNames.Add(id, name);
        }
        this.goalNames = goalNames;

        Dictionary<string, ExperienceTypeData> experienceTypes = new Dictionary<string, ExperienceTypeData>();
        foreach (JObject experienceType in index["experienceTypes"]!)
        {
            string id = experienceType["id"]!.ToString();
            experienceTypes.Add(id, new ExperienceTypeData(experienceType));
        }
        this.experienceTypes = experienceTypes;

        Dictionary<string, Sprite> icons = new Dictionary<string, Sprite>();
        foreach (Sprite sprite in Resources.LoadAll<Sprite>(ResourcePath + "/Graphics/Icons"))
        {
            icons.Add(sprite.name, sprite);
        }
        this.icons = icons;

        this.cardLibrary = new CardLibrary(ResourcePath + "/Cards/");
        this.player = new Player(index["startingCards"]!.ToObject<List<string>>(), (index["startingEnergy"] as JObject)!);
        List<CombatTarget> combatTargets = new List<CombatTarget>();
        TextAsset[] jsonFilesCombatTarget = Resources.LoadAll<TextAsset>(ResourcePath + "/CombatTargets/");
        foreach (TextAsset jsonFile in jsonFilesCombatTarget)
        {
            JObject jsonObject = JObject.Parse(jsonFile.text);
            CombatTarget combatTarget = new CombatTarget(jsonObject);
            combatTargets.Add(combatTarget);
        }
        this.combatTargets = combatTargets;
        List<Location> locations = new List<Location>();
        TextAsset[] jsonFilesLocations = Resources.LoadAll<TextAsset>(ResourcePath + "/Locations/");
        foreach (TextAsset jsonFile in jsonFilesLocations)
        {
            JObject jsonObject = JObject.Parse(jsonFile.text);
            Location location = new Location(jsonObject);
            locations.Add(location);
        }
        this.locations = locations;
        this.remainingRounds = 4;

        this.selectActionBackground = index["selectActionBackground"]!.ToString();
        this.checkIcon = index["checkIcon"]!.ToString();

        this.gameOverDialog = Dialog.FromJson(index["gameOverDialog"] as JArray ?? new JArray(), () =>
        {
            FadeHandler.Instance!.LoadScene("DebugScene");
        });
        this.boolFlags = new Dictionary<string, bool>();
    }

    public Player GetPlayer()
    {
        return player;
    }

    public int GetRemainingRounds()
    {
        return remainingRounds;
    }

    public void AddRemainingRounds(int amount)
    {
        remainingRounds += amount;
    }

    public CombatTarget GetCurrentCombatTarget()
    {
        if (this.currentCombatTarget == null)
        {
            throw new System.Exception("Current combat target is not set. Please set it before accessing.");
        }
        return this.currentCombatTarget;
    }

    public void SetCurrentCombatTarget(CombatTarget combatTarget)
    {
        this.currentCombatTarget = combatTarget;
    }

    public void StartRound()
    {
        if (!(this.boolFlags.TryGetValue(tutorialFlag, out bool isTutorialDone) && isTutorialDone))
        {
            this.boolFlags[tutorialFlag] = true;
            CombatTarget? tutorialTarget = combatTargets.Find(target => target.GetTargetType() == TargetType.Tutorial);
            if (tutorialTarget != null)
            {
                tutorialTarget.ExecuteAction();
                return;
            }
        }
        string selectActionText;
        if (LocalizationHelper.GetLocalization() == "de")
        {
            selectActionText = "Wähle eine Aktion";
        }
        else
        {
            selectActionText = "Select an action";
        }
        DialogHandler.Instance!.StartDialog(new DialogImage(new DialogSelect(selectActionText, new List<DialogOption>() {
            this.combatTargets[0].GetDialogOption()!,
            this.locations[0].GetDialogOption()!,
        }, SelectType.Cards, true), this.selectActionBackground));
    }

    public void EndRound()
    {
        this.triggerMessageSubscribers.Clear();
        remainingRounds--;
        if (remainingRounds <= 0)
        {
            DialogHandler.Instance!.StartDialog(this.gameOverDialog);
        }
        else
        {
            // Start a new round
            StartRound();
        }
    }

    public string GetGoalName(string id)
    {
        if (goalNames.ContainsKey(id))
        {
            return goalNames[id];
        }
        else
        {
            return "Unknown Goal";
        }
    }

    public string GetExperienceTypeName(string id)
    {
        if (experienceTypes.ContainsKey(id))
        {
            return experienceTypes[id].GetName();
        }
        else
        {
            return "Unknown Experience Type";
        }
    }

    public string GetExperienceTypeInlineIcon(string id)
    {
        if (experienceTypes.ContainsKey(id))
        {
            return experienceTypes[id].GetInlineIcon();
        }
        else
        {
            return "Unknown Experience Type";
        }
    }

    public Sprite? GetExperienceTypeIcon(string id)
    {
        return experienceTypes[id]?.GetIcon();
    }

    public Sprite? GetIcon(string id)
    {
        if (icons.ContainsKey(id))
        {
            return icons[id];
        }
        else
        {
            return null;
        }
    }

    public Sprite? GetCheckIcon()
    {
        return this.GetIcon(this.checkIcon);
    }

    public string GetCheckInlineIcon()
    {
        return "<sprite=" + this.checkIcon + ">";
    }

    public Card GetCard(string cardID)
    {
        return cardLibrary.GetCard(cardID);
    }

    public string GetResourcePath()
    {
        return resourcePath;
    }

    public void SubscribeToTriggerMessages(Action<TriggerMessage> subscriber)
    {
        this.triggerMessageSubscribers.Add(subscriber);
    }

    public void UnsubscribeFromTriggerMessages(Action<TriggerMessage> subscriber)
    {
        this.triggerMessageSubscribers.Remove(subscriber);
    }

    public void SendTriggerMessage(TriggerMessage message)
    {
        foreach (Action<TriggerMessage> subscriber in this.triggerMessageSubscribers)
        {
            subscriber(message);
        }
    }

    public CombatTarget GetCombatTarget(string id)
    {
        foreach (CombatTarget target in combatTargets)
        {
            if (target.GetID() == id)
            {
                return target;
            }
        }
        throw new System.Exception("Combat target with ID " + id + " not found.");
    }
}