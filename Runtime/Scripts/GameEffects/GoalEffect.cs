using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using UnityEngine.Localization.Settings;

#nullable enable

public class GoalEffect : GameEffect
{

    private IEffectOwner? owner;
    private AmountCalculation amountCalculation;
    private string goal;
    private CardEffectTrigger trigger;
    private EffectTarget effectTarget;
    private bool protection;

    public GoalEffect(string goal, AmountCalculation amountCalculation, IEffectOwner? owner, CardEffectTrigger trigger = CardEffectTrigger.PlayCard, EffectTarget? effectTarget = null, bool protection = false)
    {
        this.goal = goal;
        this.amountCalculation = amountCalculation;
        this.owner = owner;
        this.trigger = trigger;
        this.protection = protection;
        this.effectTarget = effectTarget ?? (protection ? EffectTarget.Self : EffectTarget.Turn);
    }

    public GoalEffect(string goal, int amount, IEffectOwner? owner, CardEffectTrigger trigger = CardEffectTrigger.PlayCard, EffectTarget? effectTarget = null, bool protection = false) : this(goal, new ConstantAmountCalculation(amount), owner, trigger, effectTarget, protection)
    {
    }

    public GoalEffect(JObject json, IEffectOwner? owner, CardEffectTrigger trigger = CardEffectTrigger.PlayCard) : this(
        json["goal"]?.ToString() ?? "unknown",
        AmountCalculation.FromJson(json["amount"]) ?? new ConstantAmountCalculation(0),
        owner,
        trigger,
        EnumHelper.ParseEnum<EffectTarget>(json["target"]?.ToString()),
        json["type"]?.ToString() == "protection"
    )
    {
    }

    public override void applyEffect(Enemy? target = null)
    {
        List<GoalManager> goalManagers = new List<GoalManager>();
        switch (this.effectTarget)
        {
            case EffectTarget.Turn:
                if (CombatHandler.instance == null)
                {
                    throw new System.Exception("CombatHandler instance is required for GoalEffect with Turn target");
                }
                goalManagers.Add(CombatHandler.instance!.GetGoalManager());
                break;

            case EffectTarget.SingleEnemy:
                if (target == null)
                {
                    throw new System.Exception("Target enemy is required for GoalEffect with SingleEnemy target");
                }
                goalManagers.Add(target.GetGoalManager());
                break;

            case EffectTarget.AllEnemies:
                if (CombatHandler.instance == null)
                {
                    throw new System.Exception("CombatHandler instance is required for GoalEffect with AllEnemies target");
                }
                foreach (Enemy enemy in CombatHandler.instance!.GetEnemies() ?? new List<Enemy>())
                {
                    goalManagers.Add(enemy.GetGoalManager());
                }
                break;

            case EffectTarget.Self:
                if (owner is Enemy enemyOwner)
                {
                    goalManagers.Add(enemyOwner.GetGoalManager());
                }
                else
                {
                    goalManagers.Add(Game.Instance!.GetPlayer().GetGoalManager());
                }
                break;

            case EffectTarget.Player:
                goalManagers.Add(Game.Instance!.GetPlayer().GetGoalManager());
                break;
        }

        foreach (GoalManager goalManager in goalManagers)
        {
            int amount = this.amountCalculation.GetValue(this.owner);
            if (this.protection)
            {
                goalManager.AddProtection(goal, amount);
            }
            else {
                goalManager.AddGoal(goal, amount, this.trigger);
            }
        }
    }

    public override GameEffect Clone(IEffectOwner? newOwner)
    {
        return new GoalEffect(this.goal, this.amountCalculation, newOwner ?? this.owner, this.trigger, this.effectTarget, this.protection);
    }

    private async Task<string> GetDescription(string tableEntry, string? overridePrefix = null, string? overrideSuffix = null)
    {
        bool negate = this.IsNegated();
        GoalUsageType type = this.protection ?
                            GoalUsageType.Protection :
                            negate ?
                                GoalUsageType.Negative :
                                GoalUsageType.Default;
        GoalData? goalData = Game.Instance.GetGoalData(this.goal);
        if (goalData == null)
        {
            throw new System.Exception($"No GoalData found for goal {this.goal}");
        }

        if ((this.amountCalculation.GetValue(this.owner) < 0) && !this.protection)
        {
            string? negativeIcon = goalData.GetInlineIcon(GoalUsageType.Negative);
            if (negativeIcon != null)
            {
                negate = true;
                type = GoalUsageType.Negative;
            }
        }
        return await AsyncHelper.HandleToTask(LocalizationSettings.StringDatabase.GetLocalizedStringAsync("CardStrings", tableEntry,
            arguments: new Dictionary<string, object> {
                { "prefix", overridePrefix ?? (await this.amountCalculation.GetDescriptionPrefix(this.owner, negate)) },
                { "goal", Game.Instance.GetGoalInlineIcon(this.goal, type) },
                { "suffix", overrideSuffix ?? (await this.amountCalculation.GetDescriptionSuffix(this.owner)) }
            }
        ));
    }
    
    public override async Task<string?> GetInternalIconDescription()
    {
        bool negate = this.IsNegated();
        return await GetDescription(
            "GoalIcon",
            overridePrefix: await this.amountCalculation.GetDescriptionPrefixIcon(this.owner, negate),
            overrideSuffix: await this.amountCalculation.GetDescriptionSuffixIcon(this.owner)
        );
    }

    public override Task<string> getDescription()
    {
        return GetDescription("Goal");
    }

    public override Task<string> getTriggerDescription()
    {
        return GetDescription("GoalTrigger");
    }

    public override bool requiresTarget()
    {
        return this.effectTarget == EffectTarget.SingleEnemy;
    }

    private bool IsNegated()
    {
        if ((this.amountCalculation.GetValue(this.owner) < 0) && !this.protection)
        {
            string? negativeIcon = Game.Instance.GetGoalInlineIcon(this.goal, GoalUsageType.Negative);
            if (negativeIcon != null)
            {
                return true;
            }
        }
        return false;
    }
}