using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using UnityEngine;

public abstract class SkillCharge : Clonable<SkillCharge>
{
    public static SkillCharge FromJson(JObject json, Skill skill)
    {
        string type = json["type"]!.ToString();
        switch (type)
        {
            case "playCards":
                return new PlayCardsCharge(json, skill);
            default:
                throw new System.Exception($"Unknown charge type: {type}");
        }
    }

    protected int progress;
    protected int progressForCharge;
    protected Skill skill;

    public SkillCharge(int progressForCharge, Skill skill, int progress = 0)
    {
        this.progress = progress;
        this.progressForCharge = progressForCharge;
        this.skill = skill;
    }

    public SkillCharge(JObject json, Skill skill)
    {
        this.progress = 0;
        this.progressForCharge = json["progressForCharge"]!.ToObject<int>();
        this.skill = skill;
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

    public int GetTotalProgress()
    {
        return this.progress;
    }

    public int GetCurrentProgress()
    {
        // Each charge requires double the progress of the last
        int multiplier = 1;
        int progress = this.progress;
        while (progress >= (progressForCharge * multiplier))
        {
            progress -= progressForCharge * multiplier;
            multiplier *= 2;
        }
        return progress;
    }

    public int GetMaxProgress()
    {
        return progressForCharge * (int)System.Math.Pow(2, GetCharges());
    }

    public float GetProgressPercentual()
    {
        return (float)GetCurrentProgress() / GetMaxProgress();
    }

    public void AddProgress(int amount)
    {
        this.progress += amount;
        Game.Instance.SendTriggerMessage(new TriggerMessage(TriggerType.SkillProgressChanged, new TriggerMessageData(skill: this.skill, amount: amount)));
    }

    public abstract Task<string> GetTextDescription();
    public abstract SkillCharge Clone();
    protected abstract void HandleMessage(TriggerMessage triggerMessage);
}