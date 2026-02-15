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

    public GoalEffect(string goal, AmountCalculation amountCalculation, IEffectOwner? owner, CardEffectTrigger trigger = CardEffectTrigger.PlayCard, EffectTarget effectTarget = EffectTarget.Turn)
    {
        this.goal = goal;
        this.amountCalculation = amountCalculation;
        this.owner = owner;
        this.trigger = trigger;
        this.effectTarget = effectTarget;
    }

    public GoalEffect(string goal, int amount, IEffectOwner? owner, CardEffectTrigger trigger = CardEffectTrigger.PlayCard, EffectTarget effectTarget = EffectTarget.Turn) : this(goal, new ConstantAmountCalculation(amount), owner, trigger, effectTarget)
    {
    }

    public GoalEffect(JObject json, IEffectOwner? owner, CardEffectTrigger trigger = CardEffectTrigger.PlayCard) : this(
        json["goal"]?.ToString() ?? "unknown",
        AmountCalculation.FromJson(json["amount"]) ?? new ConstantAmountCalculation(0),
        owner,
        trigger,
        EnumHelper.ParseEnum<EffectTarget>(json["target"]?.ToString()) ?? EffectTarget.Turn
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
                    throw new System.Exception("Owner must be an Enemy for GoalEffect with Self target");
                }
                break;
        }

        foreach (GoalManager goalManager in goalManagers)
        {
            goalManager.AddGoal(goal, this.amountCalculation.GetValue(this.owner), this.trigger);
        }
    }

    public override GameEffect Clone(IEffectOwner? newOwner)
    {
        return new GoalEffect(this.goal, this.amountCalculation, newOwner ?? this.owner, this.trigger, this.effectTarget);
    }

    private async Task<string> GetDescription(string tableEntry, string? overridePrefix = null, string? overrideSuffix = null)
    {
        return await AsyncHelper.HandleToTask(LocalizationSettings.StringDatabase.GetLocalizedStringAsync("CardStrings", tableEntry,
            arguments: new Dictionary<string, object> {
                { "prefix", overridePrefix ?? (await this.amountCalculation.GetDescriptionPrefix(this.owner)) },
                { "goal", Game.Instance.GetGoalInlineIcon(this.goal) },
                { "suffix", overrideSuffix ?? (await this.amountCalculation.GetDescriptionSuffix(this.owner)) }
            }
        ));
    }
    
    public override async Task<string?> GetInternalIconDescription()
    {
        return await GetDescription(
            "GoalIcon",
            overridePrefix: await this.amountCalculation.GetDescriptionPrefixIcon(this.owner),
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
}