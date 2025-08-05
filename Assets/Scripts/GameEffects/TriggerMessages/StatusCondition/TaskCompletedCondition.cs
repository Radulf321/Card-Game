using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;

public class TaskCompletedCondition : StatusCondition
{
    private string taskID;

    public TaskCompletedCondition(JObject json)
    {
        this.taskID = json["task"]!.ToString();
    }

    public override bool IsFulfilled()
    {
        return Game.Instance.GetTaskManager().IsTaskCompleted(this.taskID);
    }
}