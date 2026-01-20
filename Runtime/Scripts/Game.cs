using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using UnityEngine;

#nullable enable
class Game
{
    public static Game Instance { get; private set; } = new Game();

    public static readonly string saveGameKey = "SaveGame";
    public static readonly string tutorialDoneKey = "TutorialDone";
    public static readonly string permanentFlagsKey = "PermanentFlags";

    private Player player;

    private Dictionary<string, NamedIconData> goals;

    private Dictionary<string, NamedIconData> experienceTypes;
    private Dictionary<string, CurrencyData> currencies;

    private Dictionary<string, Sprite> icons;

    private CardLibrary cardLibrary;
    private SkillLibrary skillLibrary;
    private EnemyLibrary enemyLibrary;

    private string resourcePath;

    private string selectActionBackground;
    private string checkIcon;
    private Dialog? gameOverDialog;
    private CombatTarget? currentCombatTarget;
    private List<Action<TriggerMessage>> triggerMessageSubscribers = new List<Action<TriggerMessage>>();
    private bool tutorialDone;
    private Dictionary<FlagValidity, FlagDictionary> flagDictionaries;
    private HashSet<Modifier> modifiers;
    private HashSet<Buff> buffs;
    private EquipmentManager equipmentManager;
    private TaskManager taskManager;
    private CharacterManager characterManager;
    private List<TriggerAction> triggerActions;

    private bool startNextRound = true;

    public Game()
    {
        this.currencies = new Dictionary<string, CurrencyData>();
        this.player = new Player();
        this.goals = new Dictionary<string, NamedIconData>();
        this.experienceTypes = new Dictionary<string, NamedIconData>();
        this.icons = new Dictionary<string, Sprite>();
        this.cardLibrary = new CardLibrary();
        this.skillLibrary = new SkillLibrary();
        this.enemyLibrary = new EnemyLibrary();
        this.resourcePath = "";
        this.selectActionBackground = "";
        this.checkIcon = "";
        this.gameOverDialog = new DialogText("Dummy");
        this.tutorialDone = PlayerPrefs.GetInt(this.resourcePath + Game.tutorialDoneKey, 0) == 1;
        this.flagDictionaries = new Dictionary<FlagValidity, FlagDictionary>();
        this.modifiers = new HashSet<Modifier>();
        this.buffs = new HashSet<Buff>();
        this.equipmentManager = new EquipmentManager();
        this.taskManager = new TaskManager();
        this.characterManager = new CharacterManager();
        this.triggerActions = new List<TriggerAction>();
    }

    public Game(string ResourcePath)
    {
        Instance = this;
        this.resourcePath = ResourcePath;
        TMPro.TMP_Settings.defaultSpriteAsset = Resources.Load<TMPro.TMP_SpriteAsset>(ResourcePath + "/Graphics/Icons");
        JObject index = JObject.Parse(Resources.Load<TextAsset>(ResourcePath + "/Index").text);
        Dictionary<string, NamedIconData> goals = new Dictionary<string, NamedIconData>();
        foreach (JObject goal in index["goals"]!)
        {
            string id = goal["id"]!.ToString();
            NamedIconData data = new NamedIconData(goal);
            goals.Add(id, data);
        }
        this.goals = goals;

        Dictionary<string, NamedIconData> experienceTypes = new Dictionary<string, NamedIconData>();
        foreach (JObject experienceType in index["experienceTypes"]!)
        {
            string id = experienceType["id"]!.ToString();
            experienceTypes.Add(id, new NamedIconData(experienceType));
        }
        this.experienceTypes = experienceTypes;

        Dictionary<string, CurrencyData> currencies = new Dictionary<string, CurrencyData>();
        foreach (JObject currency in index["currencies"]!)
        {
            string id = currency["id"]!.ToString();
            currencies.Add(id, new CurrencyData(currency));
        }
        this.currencies = currencies;

        Dictionary<string, Sprite> icons = new Dictionary<string, Sprite>();
        foreach (Sprite sprite in Resources.LoadAll<Sprite>(ResourcePath + "/Graphics/Icons"))
        {
            icons.Add(sprite.name, sprite);
        }
        this.icons = icons;

        this.cardLibrary = new CardLibrary(ResourcePath + "/Cards/");
        this.skillLibrary = new SkillLibrary(ResourcePath + "/Skills/");
        this.enemyLibrary = new EnemyLibrary(ResourcePath + "/Enemies/");
        this.player = new Player();

        this.selectActionBackground = index["selectActionBackground"]!.ToString();
        this.checkIcon = index["checkIcon"]!.ToString();

        List<TriggerAction> triggerActions = new List<TriggerAction>();
        foreach (JObject triggerActionData in index["triggerActions"] ?? new JArray())
        {
            triggerActions.Add(new TriggerAction(triggerActionData, unregisterOnEndRound: false));
        }
        this.triggerActions = triggerActions;
        this.gameOverDialog = Dialog.FromJson(index["gameOverDialog"] as JArray ?? new JArray());
        this.tutorialDone = PlayerPrefs.GetInt(this.resourcePath + Game.tutorialDoneKey, 0) == 1;
        this.flagDictionaries = new Dictionary<FlagValidity, FlagDictionary>();
        if (PlayerPrefs.HasKey(this.resourcePath + permanentFlagsKey))
        {
            JObject json = JObject.Parse(PlayerPrefs.GetString(this.resourcePath + permanentFlagsKey));
            this.flagDictionaries[FlagValidity.Permanent] = new FlagDictionary(json);
        }
        this.modifiers = new HashSet<Modifier>();
        this.buffs = new HashSet<Buff>();
        this.equipmentManager = new EquipmentManager(ResourcePath, (index["equipment"] as JObject)!);
        this.taskManager = new TaskManager(ResourcePath, (index["gameEnd"] as JObject)!);
        this.characterManager = new CharacterManager(ResourcePath);
    }

    public Player GetPlayer()
    {
        return player;
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
        foreach (TriggerAction triggerAction in this.triggerActions)
        {
            triggerAction.Subscribe();
        }
        this.taskManager.Initialize();
        this.equipmentManager.HandlePreparation(StartRound);
    }

    public void ContinueGame()
    {
        if (!PlayerPrefs.HasKey(this.resourcePath + Game.saveGameKey))
        {
            throw new System.Exception("No save game found");
        }
        foreach (TriggerAction triggerAction in this.triggerActions)
        {
            triggerAction.Subscribe();
        }
        JObject saveData = JObject.Parse(PlayerPrefs.GetString(this.resourcePath + Game.saveGameKey));
        this.player = new Player(saveData["player"]?.ToObject<JObject>() ?? new JObject());
        this.flagDictionaries[FlagValidity.Game] = new FlagDictionary(saveData["flagDictionary"]?.ToObject<JObject>() ?? new JObject());
        foreach (JObject buffData in saveData["buffs"]?.ToObject<JArray>() ?? new JArray())
        {
            // Buff constructor registers the modifier automatically
            new Buff(buffData);
        }
        this.equipmentManager.LoadFromJson(saveData["equipmentManager"] as JObject ?? new JObject());
        this.taskManager.LoadFromJson(saveData["taskManager"] as JObject ?? new JObject());
        this.characterManager.LoadFromJson(saveData["characterManager"] as JObject ?? new JObject());
        StartRound();
    }

    public void StartRound()
    {
        SaveGame();
        if (!this.tutorialDone)
        {
            this.tutorialDone = true;
            PlayerPrefs.SetInt(this.resourcePath + Game.tutorialDoneKey, 1);
            PlayerPrefs.Save();
            CombatTarget? tutorialTarget = characterManager.GetTutorialTarget();
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

        _ = DialogHandler.Instance!.StartDialog(new DialogImage(new DialogSelect(selectActionText, characterManager.GetRoundOptions(), SelectType.Cards, true), this.selectActionBackground), onFinish: () =>
        {
            string targetID = GetFlagString(FlagValidity.Dialog, CombatTarget.CurrentTargetKey)!;
            characterManager.GetActionCharacter(targetID)?.ExecuteAction();
        });
    }

    public void EndRound()
    {
        this.startNextRound = true;
        // The trigger message could trigger something that prevents the next round from starting
        SendTriggerMessage(new TriggerMessage(TriggerType.EndRound));
        // Start a new round
        if (this.startNextRound) {
            StartRound();
        }
    }

    public void LooseGame()
    {
        this.startNextRound = false;
        PlayerPrefs.DeleteKey(this.resourcePath + Game.saveGameKey);
        PlayerPrefs.Save();
        _ = DialogHandler.Instance!.StartDialog(this.gameOverDialog, onFinish: () =>
            {
                this.taskManager.EndGame();
                FadeHandler.Instance!.LoadScene("GameEndScene");
            }
        );
    }

    public string GetGoalName(string id)
    {
        if (goals.ContainsKey(id))
        {
            return goals[id].GetName();
        }
        else
        {
            return "Unknown Goal";
        }
    }

    public string GetGoalInlineIcon(string id)
    {
        if (goals.ContainsKey(id))
        {
            return goals[id].GetInlineIcon();
        }
        else
        {
            return "Unknown Goal";
        }
    }

    public Sprite? GetGoalIcon(string id)
    {
        return goals[id]?.GetIcon();
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

    public CurrencyData? GetCurrencyData(string id)
    {
        if (currencies.ContainsKey(id))
        {
            return currencies[id];
        }
        else
        {
            return null;
        }
    }

    public List<string> GetCurrencies()
    {
        return this.currencies.Keys.ToList();
    }

    public string GetCurrencyName(string id)
    {
        return GetCurrencyData(id)?.GetName() ?? "Unknown Currency";
    }

    public string GetCurrencyInlineIcon(string id)
    {
        return GetCurrencyData(id)?.GetInlineIcon() ?? "Unknown Currency";
    }

    public Sprite? GetCurrencyIcon(string id)
    {
        return GetCurrencyData(id)?.GetIcon();
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

    public List<Card> GetAllCards()
    {
        return cardLibrary.GetAllCards();
    }

    public Skill GetSkill(string skillID)
    {
        return skillLibrary.GetSkill(skillID);
    }

    public Enemy GetEnemy(string enemyID)
    {
        return enemyLibrary.GetEnemy(enemyID);
    }

    public string GetResourcePath()
    {
        return resourcePath;
    }

    public void SubscribeToTriggerMessages(Action<TriggerMessage> subscriber)
    {
        if (!this.triggerMessageSubscribers.Contains(subscriber))
        {
            this.triggerMessageSubscribers.Add(subscriber);
        }
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

        if ((message.GetTriggerType() == TriggerType.EndDialog) &&
            this.flagDictionaries.ContainsKey(FlagValidity.Dialog))
        {
            // Clear dialog flags after dialog ends
            this.flagDictionaries[FlagValidity.Dialog].Clear();
        }
    }

    public void SetFlag(FlagValidity validity, string key, object value)
    {
        if (!flagDictionaries.ContainsKey(validity))
        {
            flagDictionaries[validity] = new FlagDictionary();
        }
        flagDictionaries[validity].SetValue(key, value);

        if (validity == FlagValidity.Permanent)
        {
            PlayerPrefs.SetString(this.resourcePath + permanentFlagsKey, flagDictionaries[FlagValidity.Permanent].ToJson().ToString());
            PlayerPrefs.Save();
        }
    }

    public bool? GetFlagBool(FlagValidity? validity, string key)
    {
        foreach (FlagValidity innerValidity in Enum.GetValues(typeof(FlagValidity)))
        {
            if (validity.HasValue && innerValidity != validity.Value)
            {
                continue;
            }
            if (flagDictionaries.ContainsKey(innerValidity))
            {
                bool? value = flagDictionaries[innerValidity].GetBoolValue(key);
                if (value.HasValue)
                {
                    return value;
                }
            }
        }
        return null;
    }

    public int? GetFlagInt(FlagValidity? validity, string key)
    {
        foreach (FlagValidity innerValidity in Enum.GetValues(typeof(FlagValidity)))
        {
            if (validity.HasValue && innerValidity != validity.Value)
            {
                continue;
            }
            if (flagDictionaries.ContainsKey(innerValidity))
            {
                int? value = flagDictionaries[innerValidity].GetIntValue(key);
                if (value.HasValue)
                {
                    return value;
                }
            }
        }
        return null;
    }

    public float? GetFlagFloat(FlagValidity? validity, string key)
    {
        foreach (FlagValidity innerValidity in Enum.GetValues(typeof(FlagValidity)))
        {
            if (validity.HasValue && innerValidity != validity.Value)
            {
                continue;
            }
            if (flagDictionaries.ContainsKey(innerValidity))
            {
                float? value = flagDictionaries[innerValidity].GetFloatValue(key);
                if (value.HasValue)
                {
                    return value;
                }
            }
        }
        return null;
    }

    public string? GetFlagString(FlagValidity? validity, string key)
    {
        foreach (FlagValidity innerValidity in Enum.GetValues(typeof(FlagValidity)))
        {
            if (validity.HasValue && innerValidity != validity.Value)
            {
                continue;
            }
            if (flagDictionaries.ContainsKey(innerValidity))
            {
                string? value = flagDictionaries[innerValidity].GetStringValue(key);
                if (value != null)
                {
                    return value;
                }
            }
        }
        return null;
    }

    public object? GetFlag(FlagValidity? validity, string key)
    {

        bool? boolValue = GetFlagBool(validity, key);
        if (boolValue.HasValue)
        {
            return boolValue.Value;
        }

        int? intValue = GetFlagInt(validity, key);
        if (intValue.HasValue)
        {
            return intValue.Value;
        }

        float? floatValue = GetFlagFloat(validity, key);
        if (floatValue.HasValue)
        {
            return floatValue.Value;
        }

        string? stringValue = GetFlagString(validity, key);
        if (stringValue != null)
        {
            return stringValue;
        }

        return null;
    }

    public void RegisterBuff(Buff buff)
    {
        this.buffs.Add(buff);
        RegisterModifier(buff.GetModifier());
    }

    public void UnRegisterBuff(Buff buff)
    {
        this.buffs.Remove(buff);
        UnRegisterModifier(buff.GetModifier());
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

    public CharacterManager GetCharacterManager()
    {
        return this.characterManager;
    }

    private void SaveGame()
    {
        JObject saveData = new JObject
        {
            ["player"] = this.player.ToJson(),
            ["flagDictionary"] = this.flagDictionaries.ContainsKey(FlagValidity.Game) ?
                this.flagDictionaries[FlagValidity.Game].ToJson() : new JObject(),
            ["buffs"] = new JArray(this.buffs.Select(buff => buff.ToJson())),
            ["equipmentManager"] = this.equipmentManager.SaveToJson(),
            ["taskManager"] = this.taskManager.SaveToJson(),
            ["characterManager"] = this.characterManager.SaveToJson()
        };
        PlayerPrefs.SetString(this.resourcePath + Game.saveGameKey, saveData.ToString());
        PlayerPrefs.Save();
    }
}