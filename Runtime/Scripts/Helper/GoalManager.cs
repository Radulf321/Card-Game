#nullable enable

using System;
using System.Collections.Generic;

public class GoalManager : IClonable<GoalManager>
{
    private Dictionary<string, int> goals;
    private Dictionary<string, int> goalsThisTurn;
    private Action? onGoalAdded;

    public GoalManager()
    {
        this.goals = new Dictionary<string, int>();
        this.goalsThisTurn = new Dictionary<string, int>();
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
            Game.Instance.SendTriggerMessage(new TriggerMessage(TriggerType.AddGoal, new TriggerMessageData(amount: amount, goal: goal)));
        }
        this.onGoalAdded?.Invoke();
        CombatHandler.instance?.updateView();
    }

    public void ResetGoalsThisTurn()
    {
        goalsThisTurn.Clear();
    }

    public GoalManager Clone()
    {
        GoalManager clonedGoalManager = new GoalManager();
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
}
