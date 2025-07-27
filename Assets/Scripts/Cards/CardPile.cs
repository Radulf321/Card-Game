#nullable enable

using System;
using System.Collections.Generic;

public enum DiscardType
{
    AfterPlay,
    Discard,
}

public class CardPile
{
    public List<Card> deck;
    public List<Card> hand;
    public List<Card> discardPile;
    public List<Card> inPlay;

    public CardPile(List<Card> deck)
    {
        this.deck = new List<Card>(deck);
        this.hand = new List<Card>();
        this.discardPile = new List<Card>();
        this.inPlay = new List<Card>();
    }

    public CardPile() : this(new List<Card>())
    {

    }

    public void DrawCard(CardEffectTrigger? trigger = null)
    {
        DrawCards(1, trigger);
    }

    public void DrawCards(int numberOfCards, CardEffectTrigger? trigger = null)
    {
        for (int i = 0; i < numberOfCards; i++)
        {
            DrawSingleCard();
        }
        CombatHandler.instance?.updateView();
        Game.Instance.SendTriggerMessage(new TriggerMessage(type: TriggerType.DrawCards, data: new TriggerMessageData(amount: numberOfCards, trigger: trigger)));
    }

    public List<Card> GetHand()
    {
        return this.hand;
    }

    public List<Card> GetInPlay()
    {
        return this.inPlay;
    }

    public void ShuffleDeck()
    {
        System.Random rand = new System.Random();
        List<Card> newDeck = new List<Card>();
        while (deck.Count > 0)
        {
            int index = rand.Next(deck.Count);
            newDeck.Add(deck[index]);
            deck.RemoveAt(index);
        }
        this.deck = newDeck;
    }

    public void DiscardCard(Card card, bool updateView = true, DiscardType type = DiscardType.AfterPlay)
    {
        if (hand.Contains(card) || inPlay.Contains(card))
        {
            hand.Remove(card);
            inPlay.Remove(card);
            discardPile.Add(card);
            if (updateView)
            {
                CombatHandler.instance?.updateView();
            }
        }
        else
        {
            throw new Exception("Card not in hand.");
        }
        Game.Instance.SendTriggerMessage(new TriggerMessage(TriggerType.DiscardCard, new TriggerMessageData(card: card, discardType: type)));

    }

    public void AddCardToPlay(Card card)
    {
        hand.Remove(card);
        inPlay.Add(card);
        CombatHandler.instance?.updateView();
    }

    public void RemoveCard(Card card)
    {
        hand.Remove(card);
        inPlay.Remove(card);
        discardPile.Remove(card);
        deck.Remove(card);
    }

    private void DrawSingleCard()
    {
        if (deck.Count <= 0)
        {
            ShuffleDiscardIntoDeck();
        }

        if (deck.Count > 0)
        {
            Card drawnCard = deck[0];
            deck.RemoveAt(0);
            hand.Add(drawnCard);
            drawnCard.OnDraw();
        }
    }

    private void ShuffleDiscardIntoDeck()
    {
        deck.AddRange(discardPile);
        discardPile.Clear();
        ShuffleDeck();
    }
}