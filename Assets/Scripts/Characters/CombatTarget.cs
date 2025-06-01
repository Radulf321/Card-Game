using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

#nullable enable
public class CombatTarget : ActionCharacter
{
    private int level;
    private Dictionary<string, int> experience;

    private List<Talent> talents;
    private Talent? introductionTalent;

    private string combatBackgroundPath;
    private string talentBackgroundPath;
    private List<RequirementFactory> requirementFactories;
    private AmountCalculation numberOfTurnsCalculation;
    private AmountCalculation turnsWithRequirementCalculation;
    private AmountCalculation numberOfRequirementsCalculation;
    private JArray winDialogData;
    private JArray loseDialogData;

    public CombatTarget() : base()
    {
        this.level = 1;
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
    }

    public CombatTarget(JObject jsonObject) : base(jsonObject)
    {
        this.level = 1;
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
        this.numberOfTurnsCalculation = AmountCalculation.FromJson(jsonObject["combat"]?["numberOfTurns"]) ?? new LinearAmountCalculation(2, rate: 2);
        this.turnsWithRequirementCalculation = AmountCalculation.FromJson(jsonObject["combat"]?["turnsWithRequirement"]) ?? new ConstantAmountCalculation(-1);
        this.numberOfRequirementsCalculation = AmountCalculation.FromJson(jsonObject["combat"]?["numberOfRequirements"]) ?? new ConstantAmountCalculation(0);
        this.combatBackgroundPath = jsonObject["combat"]?["background"]?.ToString() ?? "Placeholder";
        this.talentBackgroundPath = jsonObject["talentBackground"]?.ToString() ?? "Placeholder";
        this.winDialogData = jsonObject["combat"]?["win"]?["dialog"] as JArray ?? new JArray();
        this.loseDialogData = jsonObject["combat"]?["lose"]?["dialog"] as JArray ?? new JArray();
    }

    public List<Turn> GenerateTurns()
    {
        List<Turn> turns = new List<Turn>();
        int numberOfTurns = this.numberOfTurnsCalculation.GetValue(this.level);
        int requirementIndex = 0;
        int nextTurnWithRequirement = this.turnsWithRequirementCalculation.GetValue(requirementIndex);
        for (int turnIndex = 0; turnIndex < numberOfTurns; turnIndex++)
        {
            List<Requirement> requirements;
            if (turnIndex == nextTurnWithRequirement)
            {
                requirementIndex++;
                nextTurnWithRequirement = this.turnsWithRequirementCalculation.GetValue(requirementIndex);
                requirements = GetRandomRequirements(turnIndex);
            }
            else
            {
                requirements = new List<Requirement>();
            }
            // turnIndex starts at zero while index for GetEnergyForTurn starts at 1
            int energy = Game.Instance.GetPlayer().GetEnergyForTurn(turnIndex + 1);
            List<CardEffect> effects = new List<CardEffect>();
            if (energy > 0)
            {
                effects.Add(new EnergyEffect(energy, maxEnergy: true));
            }
            turns.Add(new Turn(requirements: requirements, effects: effects));
        }

        return turns;
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

    protected override void ExecuteAction()
    {
        Action start = () =>
        {
            FadeHandler.Instance!.LoadScene("CombatScene");
        };
        // == false as introductionTalent could be null
        if (this.introductionTalent?.IsPurchased() == false)
        {
            DialogHandler.dialogFinish = start;
            this.introductionTalent?.Purchase();
        }
        else
        {
            start.Invoke();
        }
    }

    public void EndCombat(bool win)
    {
        if (!win)
        {
            Game.Instance.AddRemainingRounds(-1);
            DialogHandler.StartDialog(
                Dialog.FromJson(this.loseDialogData, onFinish: () =>
                {
                    Game.Instance.EndRound();
                })
            );
        }
        else
        {
            DialogHandler.StartDialog(
                Dialog.FromJson(this.winDialogData, actionGenerator: (id) =>
                {
                    return () =>
                    {
                        if (id != null)
                        {
                            this.IncreaseExperience(id, 1);
                        }
                        this.IncreaseLevel();
                        FadeHandler.Instance!.LoadScene("TalentTreeScene");
                    };
                })
            );
        }
    }

    private List<Requirement> GetRandomRequirements(int turn)
    {
        int numberOfRequirements = this.numberOfRequirementsCalculation.GetValue(turn);
        List<int> requirementIndexes = new List<int>();
        List<Requirement> requirements = new List<Requirement>();
        for (int i = 0; i < numberOfRequirements; i++)
        {
            int random;
            do
            {
                random = UnityEngine.Random.Range(0, this.requirementFactories.Count);
            } while (requirementIndexes.Contains(random));
            requirementIndexes.Add(random);
            requirements.Add(this.requirementFactories[random].CreateRequirement(turn));
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
}