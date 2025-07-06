using System.Collections.Generic;

public class CardListDisplay : CardsContainerHandler<Card>
{
    List<Card> cards = new List<Card>();

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
        return this.cards;
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

    public void SetCards(List<Card> cards)
    {
        this.cards = cards;
        this.updateView();
    }
}
