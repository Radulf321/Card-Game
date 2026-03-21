#nullable enable

using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Unity.Mathematics;

public class GoalManager : IClonable<GoalManager>
{
    private Dictionary<string, int> goals;
    private Dictionary<string, int> goalsThisTurn;
    private Dictionary<string, int> protection;
    private Action? onGoalAdded;
    private List<Requirement> requirements;

    public GoalManager(List<Requirement>? requirements = null)
    {
        this.goals = new Dictionary<string, int>();
        this.goalsThisTurn = new Dictionary<string, int>();
        this.protection = new Dictionary<string, int>();
        this.requirements = requirements ?? new List<Requirement>();
        foreach (Requirement requirement in this.requirements)
        {
            requirement.SetGoalManager(this);
        }
    }

    public GoalManager(JObject json)
    {
        this.goals = new Dictionary<string, int>();
        this.goalsThisTurn = new Dictionary<string, int>();
        this.protection = new Dictionary<string, int>();
        foreach (KeyValuePair<string, JToken?> goal in json["goals"] as JObject ?? new JObject())
        {
            this.goals[goal.Key] = goal.Value?.ToObject<int>() ?? 0;
        }
        List<Requirement> requirements = new List<Requirement>();
        foreach (JObject requirementJson in json["requirements"] as JArray ?? new JArray())
        {
            Requirement.FromJson(requirementJson, goalManager: this);
        }
        this.requirements = requirements;
    }

    public void SetOnGoalAdded(Action? onGoalAdded)
    {
        this.onGoalAdded = onGoalAdded;
    }

    public int GetGoalAmount(string goal)
    {
        if (goals.ContainsKey(goal))
        {
            return goals[goal];
        }
        return 0;
    }

    public int GetGoalAmountThisTurn(string goal)
    {
        if (goalsThisTurn.ContainsKey(goal))
        {
            return goalsThisTurn[goal];
        }
        return 0;
    }

    public int GetTotal()
    {
        int total = 0;
        foreach (KeyValuePair<string, int> goal in goals)
        {
            total += goal.Value;
        }
        return total;
    }

    public void AddGoal(string goal, int amount, CardEffectTrigger? trigger = null)
    {
        if (protection.ContainsKey(goal))
        {
            int protectionAmount = protection[goal];
            if (amount <= protectionAmount)
            {
                protection[goal] -= amount;
            }
            else
            {
                protection.Remove(goal);
            }
            
            amount = math.max(0, amount - protectionAmount);
        }
        if (goals.ContainsKey(goal))
        {
            goals[goal] += amount;
        }
        else
        {
            goals.Add(goal, amount);
        }
        if (goalsThisTurn.ContainsKey(goal))
        {
            goalsThisTurn[goal] += amount;
        }
        else
        {
            goalsThisTurn.Add(goal, amount);
        }
        if (trigger == CardEffectTrigger.PlayCard)
        {
            Game.Instance.SendTriggerMessage(new TriggerMessage(TriggerType.AddGoal, new ContextInfo(amount: amount, goal: goal)));
        }
        this.onGoalAdded?.Invoke();
        CombatHandler.instance?.updateView();
    }

    public void AddProtection(string goal, int amount)
    {
        if (protection.ContainsKey(goal))
        {
            protection[goal] += amount;
        }
        else
        {
            protection.Add(goal, amount);
        }
        Game.Instance.SendTriggerMessage(new TriggerMessage(TriggerType.AddProtection, new ContextInfo(amount: amount, goal: goal)));
    }

    public void ResetProtection()
    {
        protection.Clear();
    }

    public int GetProtection(string goal)
    {
        if (protection.ContainsKey(goal))
        {
            return protection[goal];
        }
        return 0;
    }

    public void ResetGoalsThisTurn()
    {
        goalsThisTurn.Clear();
    }

    public GoalManager Clone()
    {
        GoalManager clonedGoalManager = new GoalManager(this.requirements);
        clonedGoalManager.SetOnGoalAdded(this.onGoalAdded);
        foreach (KeyValuePair<string, int> goal in this.goals)
        {
            clonedGoalManager.goals.Add(goal.Key, goal.Value);
        }
        foreach (KeyValuePair<string, int> goal in this.goalsThisTurn)
        {
            clonedGoalManager.goalsThisTurn.Add(goal.Key, goal.Value);
        }
        return clonedGoalManager;
    }

    public bool AreRequirementsFulfilled()
    {
        foreach (Requirement requirement in requirements)
        {
            if (!requirement.IsFulfilled())
            {
                return false;
            }
        }
        return true;
    }

    public List<Requirement> GetRequirements()
    {
        return this.requirements;
    }

    public JObject ToJson()
    {
        JObject json = new JObject();
        JObject goalsObject = new JObject();
        foreach (KeyValuePair<string, int> goal in this.goals)
        {
            goalsObject[goal.Key] = goal.Value;
        }
        json["goals"] = goalsObject;
        JArray requirementsArray = new JArray();
        foreach (Requirement requirement in this.requirements)
        {
            requirementsArray.Add(requirement.ToJson());
        }
        json["requirements"] = requirementsArray;
        return json;
    }
}
