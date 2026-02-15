using UnityEngine;

#nullable enable

public class DropCardAreaHandler : MonoBehaviour
{

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Game.Instance.SubscribeToTriggerMessages(OnTriggerMessage);
        transform.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

    }

    protected virtual void OnDestroy()
    {
        Game.Instance.UnsubscribeFromTriggerMessages(OnTriggerMessage);
    }

    private void OnTriggerMessage(TriggerMessage message)
    {
        switch (message.GetTriggerType())
        {
            case TriggerType.CardDragEnd:
            case TriggerType.CardDragStart:
                transform.gameObject.SetActive((message.GetTriggerType() == TriggerType.CardDragStart) && (message.GetData().GetCard()?.RequiresTarget() == false));
                break;
            default:
                break;
        }
    }
}
