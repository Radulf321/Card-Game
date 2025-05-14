public class TotalRequirement : Requirement
{
    private int amount;

    public TotalRequirement(int amount) {
        this.amount = amount;
    }

    public override bool isFulfilled() {
        return CombatHandler.instance.getTotal() >= amount;
    }

    public override string toString()
    {
        return "Total: " + CombatHandler.instance.getTotal() + "/" + amount;
    }
}
