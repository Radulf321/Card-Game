using System.Collections.Generic;
using Newtonsoft.Json.Linq;

public class Player
{

    private List<Card> deck;
    private EnergyInfo energyInfo;

    public Player()
    {
        this.deck = new List<Card>();
        this.energyInfo = new EnergyInfo();
    }

    public Player(List<string> startingCards, JObject energyInfo)
    {
        List<Card> deck = new List<Card>();
        foreach (string cardId in startingCards)
        {
            deck.Add(Game.Instance.GetCard(cardId));
        }
        this.deck = deck;
        this.energyInfo = new EnergyInfo(energyInfo);
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
        return this.energyInfo.GetStartingEnergy();
    }

    public int GetEnergyForTurn(int turn)
    {
        return this.energyInfo.GetEnergyForTurn(turn);
    }

    public void AddEnergy(int amount, int turn)
    {
        this.energyInfo.AddEnergy(amount, turn);
    }
}