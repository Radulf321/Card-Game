using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using Unity.VisualScripting;
using UnityEngine;

#nullable enable
class Game
{
    public static Game Instance { get; private set; } = new Game();

    private static readonly string tutorialDoneKey = "TutorialDone";

    private Player player;
    private int remainingRounds;
    private List<CombatTarget> combatTargets;
    private List<Location> locations;

    private Dictionary<string, string> goalNames;

    private Dictionary<string, NamedIconData> experienceTypes;
    private Dictionary<string, NamedIconData> currencies;

    private Dictionary<string, Sprite> icons;

    private CardLibrary cardLibrary;

    private string resourcePath;

    private string selectActionBackground;
    private string checkIcon;
    private Dialog gameOverDialog;
    private CombatTarget? currentCombatTarget;
    private List<Action<TriggerMessage>> triggerMessageSubscribers = new List<Action<TriggerMessage>>();
    private bool tutorialDone;
    private Dictionary<FlagValidity, FlagDictionary> flagDictionaries;
    private HashSet<Modifier> modifiers;
    private EquipmentManager equipmentManager;
    private TaskManager taskManager;

    public Game()
    {
        this.player = new Player();
        this.remainingRounds = 0;
        this.combatTargets = new List<CombatTarget>();
        this.locations = new List<Location>();
        this.goalNames = new Dictionary<string, string>();
        this.experienceTypes = new Dictionary<string, NamedIconData>();
        this.currencies = new Dictionary<string, NamedIconData>();
        this.icons = new Dictionary<string, Sprite>();
        this.cardLibrary = new CardLibrary();
        this.resourcePath = "";
        this.selectActionBackground = "";
        this.checkIcon = "";
        this.gameOverDialog = new DialogText("Dummy");
        this.tutorialDone = PlayerPrefs.GetInt(Game.tutorialDoneKey, 0) == 1;
        this.flagDictionaries = new Dictionary<FlagValidity, FlagDictionary>();
        this.modifiers = new HashSet<Modifier>();
        this.equipmentManager = new EquipmentManager();
        this.taskManager = new TaskManager();
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
            string name = LocalizationHelper.GetLocalizedString(goal["name"] as JObject)!;
            goalNames.Add(id, name);
        }
        this.goalNames = goalNames;

        Dictionary<string, NamedIconData> experienceTypes = new Dictionary<string, NamedIconData>();
        foreach (JObject experienceType in index["experienceTypes"]!)
        {
            string id = experienceType["id"]!.ToString();
            experienceTypes.Add(id, new NamedIconData(experienceType));
        }
        this.experienceTypes = experienceTypes;

        Dictionary<string, NamedIconData> currencies = new Dictionary<string, NamedIconData>();
        foreach (JObject currency in index["currencies"]!)
        {
            string id = currency["id"]!.ToString();
            currencies.Add(id, new NamedIconData(currency));
        }
        this.currencies = currencies;

        Dictionary<string, Sprite> icons = new Dictionary<string, Sprite>();
        foreach (Sprite sprite in Resources.LoadAll<Sprite>(ResourcePath + "/Graphics/Icons"))
        {
            icons.Add(sprite.name, sprite);
        }
        this.icons = icons;

        this.cardLibrary = new CardLibrary(ResourcePath + "/Cards/");
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
        this.player = new Player();

        this.remainingRounds = 4;

        this.selectActionBackground = index["selectActionBackground"]!.ToString();
        this.checkIcon = index["checkIcon"]!.ToString();

        this.gameOverDialog = Dialog.FromJson(index["gameOverDialog"] as JArray ?? new JArray());
        this.tutorialDone = PlayerPrefs.GetInt(Game.tutorialDoneKey, 0) == 1;
        this.flagDictionaries = new Dictionary<FlagValidity, FlagDictionary>();
        this.modifiers = new HashSet<Modifier>();
        this.equipmentManager = new EquipmentManager(ResourcePath, (index["equipment"] as JObject)!);
        this.taskManager = new TaskManager(ResourcePath, (index["gameEnd"] as JObject)!);
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

    public void StartGame()
    {
        this.taskManager.Initialize();
        this.equipmentManager.HandlePreparation(StartRound);
    }

    public void StartRound()
    {
        if (!this.tutorialDone)
        {
            this.tutorialDone = true;
            PlayerPrefs.SetInt(Game.tutorialDoneKey, 1);
            PlayerPrefs.Save();
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
        List<DialogOption> options = new List<DialogOption>();
        List<List<DialogOption>> dialogOptionsByType = new List<List<DialogOption>>();
        foreach (IEnumerable<ActionCharacter> actionCharacters in new List<IEnumerable<ActionCharacter>>() { this.combatTargets.Cast<ActionCharacter>(), this.locations.Cast<ActionCharacter>() })
        {
            List<DialogOption> dialogOptionsForType = new List<DialogOption>();
            foreach (ActionCharacter actionCharacter in actionCharacters)
            {
                DialogOption? dialogOption = actionCharacter.GetDialogOption();
                if (dialogOption != null)
                {
                    dialogOptionsForType.Add(dialogOption);
                }
            }
            if (dialogOptionsForType.Count > 0)
            {
                int index = UnityEngine.Random.Range(0, dialogOptionsForType.Count);
                options.Add(dialogOptionsForType[index]);
                dialogOptionsForType.RemoveAt(index);
            }
            if (dialogOptionsForType.Count > 0)
            {
                dialogOptionsByType.Add(dialogOptionsForType);
            }
        }

        if (dialogOptionsByType.Count > 0)
        {
            int typeIndex = UnityEngine.Random.Range(0, dialogOptionsByType.Count);
            int optionIndex = UnityEngine.Random.Range(0, dialogOptionsByType[typeIndex].Count);
            options.Insert(1, dialogOptionsByType[typeIndex][optionIndex]);
        }

        _ = DialogHandler.Instance!.StartDialog(new DialogImage(new DialogSelect(selectActionText, options, SelectType.Cards, true), this.selectActionBackground), onFinish: () =>
        {
            string targetID = GetFlag<string>(FlagValidity.Dialog, CombatTarget.CurrentTargetKey)!;
            GetActionCharacter(targetID)?.ExecuteAction();
        });
    }

    public void EndRound()
    {
        SendTriggerMessage(new TriggerMessage(TriggerType.EndRound));
        remainingRounds--;
        if (remainingRounds <= 0)
        {
            _ = DialogHandler.Instance!.StartDialog(this.gameOverDialog, onFinish: () =>
        {
            this.taskManager.EndGame();
            FadeHandler.Instance!.LoadScene("GameEndScene");
        });
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

    public List<string> GetCurrencies()
    {
        return this.currencies.Keys.ToList();
    }

    public string GetCurrencyName(string id)
    {
        if (currencies.ContainsKey(id))
        {
            return currencies[id].GetName();
        }
        else
        {
            return "Unknown Experience Type";
        }
    }

    public string GetCurrencyInlineIcon(string id)
    {
        if (currencies.ContainsKey(id))
        {
            return currencies[id].GetInlineIcon();
        }
        else
        {
            return "Unknown Experience Type";
        }
    }

    public Sprite? GetCurrencyIcon(string id)
    {
        return currencies[id]?.GetIcon();
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
        // Clone list as some subscribers could unsubscribe during loop execution
        foreach (Action<TriggerMessage> subscriber in new List<Action<TriggerMessage>>(this.triggerMessageSubscribers))
        {
            subscriber(message);
        }
    }

    public CombatTarget? GetCombatTarget(string id)
    {
        foreach (CombatTarget target in combatTargets)
        {
            if (target.GetID() == id)
            {
                return target;
            }
        }
        return null;
    }

    public Location? GetLocation(string id)
    {
        foreach (Location location in locations)
        {
            if (location.GetID() == id)
            {
                return location;
            }
        }
        return null;
    }

    public ActionCharacter? GetActionCharacter(string id)
    {
        CombatTarget? combatTarget = GetCombatTarget(id);
        if (combatTarget != null)
        {
            return combatTarget;
        }
        else
        {
            return GetLocation(id);
        }
    }

    public void SetFlag(FlagValidity validity, string key, object value)
    {
        if (!flagDictionaries.ContainsKey(validity))
        {
            flagDictionaries[validity] = new FlagDictionary();
        }
        flagDictionaries[validity].SetValue(key, value);
    }

    public T? GetFlag<T>(FlagValidity validity, string key)
    {
        if (!flagDictionaries.ContainsKey(validity))
        {
            return default(T);
        }
        return flagDictionaries[validity].GetValue<T>(key);
    }

    public object? GetFlag(FlagValidity validity, string key)
    {
        if (!flagDictionaries.ContainsKey(validity))
        {
            return null;
        }
        return flagDictionaries[validity].GetValue(key);
    }

    public void RegisterModifier(Modifier modifier)
    {
        this.modifiers.Add(modifier);
    }

    public void UnRegisterModifier(Modifier modifier)
    {
        this.modifiers.Remove(modifier);
    }

    public int ApplyModifiers(ModifierType type, int baseValue, int? turn = null)
    {
        int value = baseValue;
        foreach (Modifier modifier in this.modifiers)
        {
            value = modifier.GetValue(value, type, turn);
        }
        return value;
    }

    public EquipmentManager GetEquipmentManager()
    {
        return this.equipmentManager;
    }

    public TaskManager GetTaskManager()
    {
        return this.taskManager;
    }
}