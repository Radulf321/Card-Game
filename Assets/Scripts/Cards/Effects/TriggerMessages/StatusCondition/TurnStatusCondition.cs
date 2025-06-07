using Newtonsoft.Json.Linq;

public class TurnStatusCondition : StatusCondition
{
    private int turn;

    public TurnStatusCondition(JObject json)
    {
        this.turn = json["turn"]!.ToObject<int>();
    }

    public override bool IsFulfilled()
    {
        return CombatHandler.instance.getCurrentTurnIndex() == this.turn;
    }
}