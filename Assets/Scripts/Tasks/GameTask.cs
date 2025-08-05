using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

#nullable enable
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
    private List<StatusCondition> conditions;
    private string id;

    public GameTask(string id, List<Reward>? rewards = null, List<StatusCondition>? conditions = null)
    {
        this.id = id;
        this.rewards = rewards ?? new List<Reward>();
        this.conditions = conditions ?? new List<StatusCondition>();
    }

    public GameTask(JObject json)
    {
        this.id = json["id"]?.ToString() ?? "unknownID";
        List<Reward> rewards = new List<Reward>();
        foreach (JObject rewardJson in (json["rewards"] as JArray) ?? new JArray())
        {
            rewards.Add(Reward.FromJson(rewardJson));
        }
        this.rewards = rewards;
        List<StatusCondition> conditions = new List<StatusCondition>();
        foreach (JObject conditionsJson in (json["conditions"] as JArray) ?? new JArray())
        {
            conditions.Add(StatusCondition.FromJson(conditionsJson));
        }
        this.conditions = conditions;
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

    public bool IsAvailable()
    {
        foreach (StatusCondition condition in this.conditions)
        {
            if (!condition.IsFulfilled())
            {
                return false;
            }
        }
        return true;
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