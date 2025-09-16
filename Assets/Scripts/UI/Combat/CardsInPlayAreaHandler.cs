using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardsInPlayAreaHandler : CardsContainerHandler<Card>
{

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Game.Instance.SubscribeToTriggerMessages(OnTriggerMessage);
    }

    // Update is called once per frame
    void Update()
    {

    }

    protected virtual void OnDestroy()
    {
        Game.Instance.UnsubscribeFromTriggerMessages(OnTriggerMessage);
    }

    protected override List<Card> GetCardData()
    {
        return CombatHandler.instance.getCardPile().GetInPlay();
    }

    protected override Card GetHandlerData(CardHandler cardHandler)
    {
        return cardHandler.GetCard();
    }

    protected override void SetHandlerData(CardHandler cardHandler, Card cardData)
    {
        cardHandler.SetCard(cardData);
        cardHandler.SetActive(false);
    }

    private void OnTriggerMessage(TriggerMessage message)
    {
        switch (message.GetTriggerType())
        {
            case TriggerType.CardDragEnd:
            case TriggerType.CardDragStart:
                Image image = GetComponent<Image>();
                Color color = image.color;
                color.a = (message.GetTriggerType() == TriggerType.CardDragStart) ? 0.5f : 0.0f;
                image.color = color;
                break;
            default:
                break;
        }
    }
}
