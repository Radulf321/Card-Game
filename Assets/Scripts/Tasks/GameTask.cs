using System.Collections.Generic;
using System.Threading.Tasks;
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
            case "finish":
                return new FinishTask(json);
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

    public void CollectRewards()
    {
        foreach (Reward reward in this.rewards)
        {
            reward.Collect();
        }
    }

    public List<Reward> GetRewards()
    {
        return this.rewards;
    }

    public string GetID()
    {
        return this.id;
    }

    public virtual bool IsCompleted()
    {
        int? total = this.GetTotal();
        int? progress = this.GetProgress();
        if (total == null || progress == null)
        {
            throw new System.Exception("Total or progress is null");
        }
        return progress >= total;
    }

    public virtual int? GetProgress()
    {
        return null;
    }

    public virtual int? GetTotal()
    {
        return null;
    }

    public abstract Task<string> GetDescription();
}