public class Buff
{
    private int duration;
    private Modifier modifier;

    public Buff(Modifier modifier, int duration)
    {
        this.modifier = modifier;
        this.duration = duration;
        Game.Instance.SubscribeToTriggerMessages(OnMessage);
        Game.Instance.RegisterModifier(this.modifier);
    }

    private void OnMessage(TriggerMessage message)
    {
        if (message.GetTriggerType() == TriggerType.EndRound)
        {
            UnityEngine.Debug.Log("Buff received endTurn message");
            if (duration == 0)
            {
                Game.Instance.UnsubscribeFromTriggerMessages(OnMessage);
                Game.Instance.UnRegisterModifier(this.modifier);
                UnityEngine.Debug.Log("Unregistered modifier");
            }
            else
            {
                duration--;
                UnityEngine.Debug.Log("Updated duration: " + duration);
            }
        }
    }
}