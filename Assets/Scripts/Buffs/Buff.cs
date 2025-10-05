using Newtonsoft.Json.Linq;

public class Buff
{
    private int duration;
    private Modifier modifier;

    public Buff(Modifier modifier, int duration)
    {
        this.modifier = modifier;
        this.duration = duration;
        Game.Instance.SubscribeToTriggerMessages(OnMessage);
        Game.Instance.RegisterBuff(this);
    }

    public Buff(JObject json) : this(
        modifier: new Modifier(json["modifier"] as JObject),
        duration: json["duration"]!.ToObject<int>()
    )
    {

    }

    private void OnMessage(TriggerMessage message)
    {
        if (message.GetTriggerType() == TriggerType.EndRound)
        {
            if (duration == 0)
            {
                Game.Instance.UnsubscribeFromTriggerMessages(OnMessage);
                Game.Instance.UnRegisterBuff(this);
            }
            else
            {
                duration--;
            }
        }
    }

    public Modifier GetModifier()
    {
        return this.modifier;
    }

    public JObject ToJson()
    {
        return new JObject
        {
            ["modifier"] = this.modifier.ToJson(),
            ["duration"] = this.duration
        };
    }
}