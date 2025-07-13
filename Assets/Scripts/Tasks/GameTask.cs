using System.Collections.Generic;
using Newtonsoft.Json.Linq;

public abstract class GameTask
{
    public static GameTask FromJson(JObject json)
    {
        string type = json["type"]!.ToString();
        switch (type)
        {
            case "wins":
                return new WinsTask(json);
            // Add other task types here
            default:
                throw new System.Exception($"Unknown task type: {type}");
        }
    }

    private List<Reward> rewards;
    private string id;

    public GameTask(string id, List<Reward> rewards)
    {
        this.id = id;
        this.rewards = rewards;
    }

    public GameTask(JObject json)
    {
        this.id = json["id"]!.ToString();
        List<Reward> rewards = new List<Reward>();
        foreach (JObject rewardJson in json["rewards"] as JArray)
        {
            rewards.Add(Reward.FromJson(rewardJson));
        }
        this.rewards = rewards;
    }

    public abstract bool IsCompleted();

    public void CollectRewards()
    {
        foreach (Reward reward in this.rewards)
        {
            reward.Collect();
        }
    }

    public string GetID()
    {
        return this.id;
    }
}