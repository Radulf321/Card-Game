using System.Collections.Generic;

public class Player
{

    private List<Card> deck;

    public Player()
    {
        this.deck = deck = new List<Card>();
    }

    public Player(List<string> startingCards)
    {
        List<Card> deck = new List<Card>();
        foreach (string cardId in startingCards)
        {
            deck.Add(Game.Instance.GetCard(cardId));
        }
        this.deck = deck;
    }

    public List<Card> GetDeck()
    {
        return deck;
    }

    public void AddCardToDeck(Card card)
    {
        this.deck.Add(card);
    }

    public void RemoveCardFromDeck(Card card)
    {
        this.deck.Remove(card);
    }
}