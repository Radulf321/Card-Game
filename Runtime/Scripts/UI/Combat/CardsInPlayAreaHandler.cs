using System.Collections.Generic;

public class CardsInPlayAreaHandler : CardsContainerHandler<Card>
{

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {

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
}
