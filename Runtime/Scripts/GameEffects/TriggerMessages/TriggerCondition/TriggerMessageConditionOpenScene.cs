using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

#nullable enable

public enum SceneType
{
    TalentTree,
    Preparation,
    EndGame,
}

public class TriggerMessageConditionOpenScene : TriggerMessageCondition
{
    private SceneType sceneType;

    public TriggerMessageConditionOpenScene()
    {
        this.sceneType = SceneType.TalentTree;
    }

    public TriggerMessageConditionOpenScene(JObject json) : this()
    {
        this.sceneType = EnumHelper.ParseEnum<SceneType>(json["sceneType"]?.ToString()) ?? SceneType.TalentTree;
    }

    public override bool FulfillsCondition(TriggerMessage message)
    {
        switch (this.sceneType)
        {
            case SceneType.TalentTree:
                return message.GetTriggerType() == TriggerType.TalentTree;
            case SceneType.Preparation:
                return message.GetTriggerType() == TriggerType.Preparation;
            case SceneType.EndGame:
                return message.GetTriggerType() == TriggerType.EndGame;
            default:
                return false;
        }
    }

    public override Task<string> GetDescription()
    {
        throw new System.NotImplementedException("TriggerMessageConditionTalentTree does not have a description.");
    }
}