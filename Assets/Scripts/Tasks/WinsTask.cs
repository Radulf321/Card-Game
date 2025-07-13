using System.Collections.Generic;
using Newtonsoft.Json.Linq;

public class WinsTask : GameTask
{
    private int amount;

    public WinsTask(string id, List<Reward> rewards, int amount) : base(id, rewards)
    {
        this.amount = amount;
    }

    public WinsTask(JObject json) : base(json)
    {
        this.amount = json["amount"]!.ToObject<int>();
    }

    public override bool IsCompleted()
    {
        return Game.Instance.GetTaskManager().GetTotalWins() >= this.amount;
    }
}