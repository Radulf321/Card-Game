using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

#nullable enable

public class TriggerMessageConditionTalentTree : TriggerMessageCondition
{

    public TriggerMessageConditionTalentTree()
    {
    }

    public TriggerMessageConditionTalentTree(JObject json) : this()
    {
    }

    public override bool FulfillsCondition(TriggerMessage message)
    {
        if (message.GetTriggerType() != TriggerType.TalentTree)
        {
            return false;
        }

        return true;
    }

    public override Task<string> GetDescription()
    {
        throw new System.NotImplementedException("TriggerMessageConditionTalentTree does not have a description.");
    }
}