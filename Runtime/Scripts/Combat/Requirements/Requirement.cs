using System;
using System.Threading.Tasks;

#nullable enable
abstract public class Requirement : IClonable<Requirement>
{
    protected Enemy? enemy;

    protected Requirement(Enemy? enemy = null)
    {
        this.enemy = enemy;
    }

    public void SetEnemy(Enemy? enemy)
    {
        this.enemy = enemy;
    }

    abstract public bool IsFulfilled();
    abstract public Task<string> GetDescription();
    abstract public Requirement Clone();

    protected GoalManager GetGoalManager()
    {
        GoalManager? goalManager = this.enemy?.GetGoalManager() ?? CombatHandler.instance?.GetGoalManager();
        if (goalManager == null)
        {
            throw new Exception("No GoalManager available to check Requirement");
        }
        return goalManager;
    }
}
