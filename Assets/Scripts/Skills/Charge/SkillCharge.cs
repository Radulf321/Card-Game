using Newtonsoft.Json.Linq;

public abstract class SkillCharge
{
    public static SkillCharge FromJson(JObject json)
    {
        string type = json["type"]!.ToString();
        switch (type)
        {
            case "playCards":
                return new PlayCardsCharge(json);
            default:
                throw new System.Exception($"Unknown charge type: {type}");
        }
    }

    protected int progress;
    private int progressForCharge;

    public SkillCharge(JObject json)
    {
        this.progress = 0;
        this.progressForCharge = json["progressForCharge"]!.ToObject<int>();
    }

    public void Initialize()
    {
        Game.Instance.SubscribeToTriggerMessages(this.HandleMessage);
    }

    public int GetCharges()
    {
        // Each charge requires double the progress of the last
        int multiplier = 1;
        int charges = 0;
        int progress = this.progress;
        while (progress >= (progressForCharge * multiplier))
        {
            charges++;
            progress -= progressForCharge * multiplier;
            multiplier *= 2;
        }
        return charges;
    }

    public void UseCharge()
    {
        if (progress < progressForCharge)
        {
            throw new System.Exception("Not enough progress to use a charge.");
        }
        progress -= progressForCharge;

        // Halve the remaining progress after using a charge
        progress /= 2;
    }

    public int GetProgress()
    {
        return this.progress;
    }

    public void AddProgress(int amount)
    {
        this.progress += amount;
    }

    protected abstract void HandleMessage(TriggerMessage triggerMessage);
}