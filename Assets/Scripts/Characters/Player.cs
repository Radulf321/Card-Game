using System.Collections.Generic;

public class Player
{

    private List<Card> deck;
    private Dictionary<int, int> energy;

    public Player()
    {
        this.deck = new List<Card>();
        this.energy = new Dictionary<int, int>();
    }

    public Player(List<string> startingCards, Dictionary<int, int> energy)
    {
        List<Card> deck = new List<Card>();
        foreach (string cardId in startingCards)
        {
            deck.Add(Game.Instance.GetCard(cardId));
        }
        this.deck = deck;
        this.energy = energy;
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

    public int GetStartingEnergy()
    {
        return GetEnergyForTurn(0);
    }

    public int GetEnergyForTurn(int turn)
    {
        return energy.TryGetValue(turn, out int value) ? value : 0;
    }
}