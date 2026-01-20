using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

#nullable enable

public enum TargetType
{
    Regular,
    Tutorial,
}

public class CombatTarget : ActionCharacter
{
    private int level;
    private Dictionary<string, int> experience;

    private List<Talent> talents;
    private Talent? introductionTalent;

    private string combatBackgroundPath;
    private string talentBackgroundPath;
    private List<RequirementFactory> requirementFactories;
    private List<List<RequirementFactory>> fixedRequirements;
    private AmountCalculation? numberOfTurnsCalculation;
    private AmountCalculation? turnsWithRequirementCalculation;
    private AmountCalculation? numberOfRequirementsCalculation;
    private AmountCalculation? numberOfEnemiesCalculation;
    private List<String>? possibleEnemyIDs;
    private JArray winDialogData;
    private JArray loseDialogData;
    private CardPileFactory cardPileFactory;
    private EnergyInfo? energyOverride;
    private List<TriggerAction> triggerActions;
    private TargetType targetType;

    public CombatTarget() : base("Default")
    {
        this.level = 0;
        this.experience = new Dictionary<string, int>();
        this.introductionTalent = null;
        this.talents = new List<Talent>();
        this.combatBackgroundPath = "Placeholder";
        this.requirementFactories = new List<RequirementFactory>();
        this.winDialogData = new JArray();
        this.loseDialogData = new JArray();
        this.talentBackgroundPath = "Placeholder";
        this.numberOfTurnsCalculation = new LinearAmountCalculation(2, rate: 2);
        this.turnsWithRequirementCalculation = new LinearAmountCalculation(2, rate: 2);
        this.numberOfRequirementsCalculation = new ConstantAmountCalculation(1);
        this.fixedRequirements = new List<List<RequirementFactory>>();
        this.cardPileFactory = new CardPileFactory();
        this.energyOverride = null;
        this.triggerActions = new List<TriggerAction>();
        this.targetType = TargetType.Regular;
    }

    public CombatTarget(JObject jsonObject) : base(jsonObject)
    {
        this.level = 0;
        this.experience = new Dictionary<string, int>();

        List<Talent> talents = new List<Talent>();
        foreach (JObject talentData in jsonObject["talents"] ?? new JArray())
        {
            Talent talent = new Talent(talentData, this);
            if (talentData["introduction"]?.ToObject<bool>() == true)
            {
                this.introductionTalent = talent;
            }
            talents.Add(talent);
        }
        this.talents = talents;

        List<RequirementFactory> requirementFactories = new List<RequirementFactory>();
        foreach (JObject requirementData in jsonObject["combat"]?["requirements"] ?? new JArray())
        {
            requirementFactories.Add(RequirementFactory.FromJson(requirementData, this));
        }
        this.requirementFactories = requirementFactories;
        this.numberOfTurnsCalculation = AmountCalculation.FromJson(jsonObject["combat"]?["numberOfTurns"]);
        this.turnsWithRequirementCalculation = AmountCalculation.FromJson(jsonObject["combat"]?["turnsWithRequirement"]);
        this.numberOfRequirementsCalculation = AmountCalculation.FromJson(jsonObject["combat"]?["numberOfRequirements"]);

        this.numberOfEnemiesCalculation = AmountCalculation.FromJson(jsonObject["combat"]?["numberOfEnemies"]);
        this.possibleEnemyIDs = jsonObject["combat"]?["enemies"]?.ToObject<List<string>>();

        List<List<RequirementFactory>> fixedRequirements = new List<List<RequirementFactory>>();
        foreach (JArray requirementDataForTurn in jsonObject["combat"]?["fixedRequirements"] ?? new JArray())
        {
            List<RequirementFactory> requirementFactoriesForTurn = new List<RequirementFactory>();
            foreach (JObject requirement in requirementDataForTurn)
            {
                requirementFactoriesForTurn.Add(RequirementFactory.FromJson(requirement, this));
            }
            fixedRequirements.Add(requirementFactoriesForTurn);
        }
        this.fixedRequirements = fixedRequirements;
        this.combatBackgroundPath = jsonObject["combat"]?["background"]?.ToString() ?? "Placeholder";
        this.talentBackgroundPath = jsonObject["talentBackground"]?.ToString() ?? "Placeholder";
        this.winDialogData = jsonObject["combat"]?["win"]?["dialog"] as JArray ?? new JArray();
        this.loseDialogData = jsonObject["combat"]?["lose"]?["dialog"] as JArray ?? new JArray();
        this.cardPileFactory = new CardPileFactory(jsonObject["combat"]?["override"]?["cardPile"] as JObject);

        JObject? energyObject = jsonObject["combat"]?["override"]?["energy"] as JObject;
        if (energyObject != null)
        {
            this.energyOverride = new EnergyInfo(energyObject);
        }
        else
        {
            this.energyOverride = null;
        }

        List<TriggerAction> triggerActions = new List<TriggerAction>();
        foreach (JObject triggerActionData in jsonObject["triggerActions"] ?? new JArray())
        {
            triggerActions.Add(new TriggerAction(triggerActionData));
        }
        this.triggerActions = triggerActions;
        this.targetType = EnumHelper.ParseEnum<TargetType>(jsonObject["type"]?.ToString()) ?? TargetType.Regular;
    }

    public List<Turn>? GenerateTurns()
    {
        if (this.numberOfTurnsCalculation == null)
        {
            return null;
        }
        List<Turn> turns = new List<Turn>();
        int numberOfTurns = this.numberOfTurnsCalculation.GetValue(this.level);
        int requirementIndex = 0;
        int nextTurnWithRequirement = this.turnsWithRequirementCalculation?.GetValue(requirementIndex) ?? -1;
        for (int turnIndex = 0; turnIndex < numberOfTurns; turnIndex++)
        {
            List<Requirement> requirements;
            if (turnIndex == nextTurnWithRequirement)
            {
                requirementIndex++;
                nextTurnWithRequirement = this.turnsWithRequirementCalculation!.GetValue(requirementIndex);
                requirements = GetRandomRequirements(turnIndex);
            }
            else
            {
                requirements = new List<Requirement>();
            }
            if (this.fixedRequirements.Count > turnIndex)
            {
                requirements.AddRange(this.fixedRequirements[turnIndex].ConvertAll((RequirementFactory requirementFactory) => requirementFactory.CreateRequirement(turnIndex)));
            }
            // turnIndex starts at zero while index for GetEnergyForTurn starts at 1
            int energy = this.GetEnergyForTurn(turnIndex + 1);
            List<GameEffect> effects = new List<GameEffect>();
            if (energy > 0)
            {
                effects.Add(new EnergyEffect(energy, maxEnergy: true));
            }
            turns.Add(new Turn(requirements: requirements, effects: effects));
        }

        return turns;
    }

    public List<Enemy>? GenerateEnemies()
    {
        if (this.numberOfEnemiesCalculation == null)
        {
            return null;
        }
        List<Enemy> enemies = new List<Enemy>();
        int numberOfEnemies = this.numberOfEnemiesCalculation.GetValue(this.level);

        for (int i = 0; i < numberOfEnemies; i++)
        {
            string enemyID = this.possibleEnemyIDs![UnityEngine.Random.Range(0, this.possibleEnemyIDs.Count)];
            Enemy enemy = Game.Instance!.GetEnemy(enemyID);
            enemies.Add(enemy);
        }

        return enemies;
    }

    public CardPile CreateCardPile()
    {
        return this.cardPileFactory.CreateCardPile();
    }

    public int GetLevel()
    {
        return this.level;
    }

    public void IncreaseLevel()
    {
        this.level++;
    }

    public Dictionary<string, int> GetExperience()
    {
        return this.experience;
    }

    public int GetExperience(string type)
    {
        if (this.experience.ContainsKey(type))
        {
            return this.experience[type];
        }
        else
        {
            return 0;
        }
    }

    public void IncreaseExperience(string type, int amount)
    {
        if (this.experience.ContainsKey(type))
        {
            this.experience[type] += amount;
        }
        else
        {
            this.experience.Add(type, amount);
        }
    }

    public List<Talent> GetTalents()
    {
        return this.talents;
    }

    public Talent? GetTalent(string id)
    {
        foreach (Talent talent in this.talents)
        {
            if (talent.GetID() == id)
            {
                return talent;
            }
        }
        return null;
    }

    public override void ExecuteAction()
    {
        Action start = () =>
        {
            Game.Instance!.SetCurrentCombatTarget(this);
            foreach (TriggerAction triggerAction in this.triggerActions)
            {
                triggerAction.Subscribe();
            }
            FadeHandler.Instance!.LoadScene("CombatScene");
        };
        // == false as introductionTalent could be null
        if (this.introductionTalent?.IsPurchased() == false)
        {
            this.introductionTalent?.Purchase(start);
        }
        else
        {
            start.Invoke();
        }
    }

    public async Task EndCombat(bool win)
    {
        Game.Instance.SendTriggerMessage(new TriggerMessage(TriggerType.EndCombat, new TriggerMessageData(success: win, combatTarget: this)));
        Dialog? dialog;
        Action onFinish;
        if (!win)
        {
            dialog = Dialog.FromJson(this.loseDialogData);
            onFinish = () =>
                    {
                        Game.Instance.EndRound();
                    };
        }
        else
        {
            dialog = Dialog.FromJson(this.winDialogData);
            onFinish = () =>
                {
                    this.IncreaseLevel();
                    FadeHandler.Instance!.LoadScene("TalentTreeScene");
                };
        }
        if (dialog == null)
        {
            onFinish();
        }
        else
        {
            await DialogHandler.Instance!.StartDialog(
                dialog, onFinish: onFinish
            );
        }
    }

    private List<Requirement> GetRandomRequirements(int turn)
    {
        int numberOfRequirements = this.numberOfRequirementsCalculation?.GetValue(turn) ?? 0;
        List<RequirementFactory> usedFactories = new List<RequirementFactory>();
        List<Requirement> requirements = new List<Requirement>();
        for (int i = 0; i < numberOfRequirements; i++)
        {
            List<RequirementFactory> validRequirements = this.requirementFactories.Where(factory => factory.IsValid(i, turn)).ToList();
            int random;
            do
            {
                random = UnityEngine.Random.Range(0, validRequirements.Count);
            } while (usedFactories.Contains(validRequirements[random]));
            usedFactories.Add(validRequirements[random]);
            requirements.Add(validRequirements[random].CreateRequirement(turn));
        }

        return requirements;
    }

    public string GetCombatBackgroundPath()
    {
        return Game.Instance!.GetResourcePath() + "/Graphics/Backgrounds/" + this.combatBackgroundPath;
    }

    public string GetTalentBackgroundPath()
    {
        return Game.Instance!.GetResourcePath() + "/Graphics/Backgrounds/" + this.talentBackgroundPath;
    }

    public int GetStartingEnergy()
    {
        return this.energyOverride?.GetStartingEnergy() ?? Game.Instance!.GetPlayer().GetStartingEnergy();
    }

    public int GetEnergyForTurn(int turn)
    {
        return this.energyOverride?.GetEnergyForTurn(turn) ?? Game.Instance!.GetPlayer().GetEnergyForTurn(turn);
    }

    public TargetType GetTargetType()
    {
        return this.targetType;
    }
}