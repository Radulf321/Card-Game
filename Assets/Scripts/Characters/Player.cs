using System.Collections.Generic;

public class Player
{

    private List<Card> deck;

    public Player()
    {
        this.deck = deck = new List<Card>() {
            CardLibrary.getCard(CardType.Flirt),
            CardLibrary.getCard(CardType.Flirt),
            CardLibrary.getCard(CardType.Flirt),
            CardLibrary.getCard(CardType.Chat),
            CardLibrary.getCard(CardType.Chat),
            CardLibrary.getCard(CardType.SweetTalk),
            CardLibrary.getCard(CardType.SweetTalk),
            CardLibrary.getCard(CardType.Compliment),
            CardLibrary.getCard(CardType.EnergyBurst),
            CardLibrary.getCard(CardType.RiskyPlay),
        };
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
}