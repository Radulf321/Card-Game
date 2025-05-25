public class GoalRequirement : Requirement
{
    private string goal;
    private int amount;

    public GoalRequirement(string subject, int amount) {
        this.goal = subject;
        this.amount = amount;
    }

    public override bool isFulfilled() {
        return CombatHandler.instance.getGoalAmount(this.goal) >= this.amount;
    }

    public override string toString()
    {
        return Game.Instance.GetGoalName(this.goal) + ": " + CombatHandler.instance.getGoalAmount(this.goal) + "/" + this.amount;
    }
}
