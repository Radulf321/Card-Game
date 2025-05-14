using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CardPile
{
    public List<Card> deck;
    public List<Card> hand;
    public List<Card> discardPile;
    
    public CardPile(List<Card> deck) {
        this.deck = deck;
        this.hand = new List<Card>();
        this.discardPile = new List<Card>();
    }

    public void DrawCard(bool updateView = true) {
        if (deck.Count <= 0) {
            ShuffleDiscardIntoDeck();
        }

        if (deck.Count > 0) {
            Card drawnCard = deck[0];
            deck.RemoveAt(0);
            hand.Add(drawnCard);
        }

        if (updateView) {
            CombatHandler.instance.updateView();
        }
    }

    public void DrawCards(int numberOfCards) {
        for (int i = 0; i < numberOfCards; i++) {
            DrawCard(false);
        }
        CombatHandler.instance.updateView();
    }

    public void DiscardRandomCards(int numberOfCards) {
        for (int i = 0; i < numberOfCards; i++) {
            if (hand.Count <= 0) {
                throw new Exception("No cards to discard.");
            }
            int index = UnityEngine.Random.Range(0, hand.Count);
            DiscardCard(hand[index], false);
        }
        CombatHandler.instance.updateView();
    }

    private void ShuffleDiscardIntoDeck()
    {
        deck.AddRange(discardPile);
        discardPile.Clear();
        ShuffleDeck();
    }

    public List<Card> GetHand() {
        return this.hand;
    }

    public void ShuffleDeck() {
        System.Random rand = new System.Random();
        List<Card> newDeck = new List<Card>();
        while (deck.Count > 0) {
            int index = rand.Next(deck.Count);
            newDeck.Add(deck[index]);
            deck.RemoveAt(index);
        }
        this.deck = newDeck;
    }

    public void DiscardCard(Card card, bool updateView = true) {
        if (hand.Contains(card)) {
            hand.Remove(card);
            discardPile.Add(card);
            if (updateView) {
                CombatHandler.instance.updateView();
            }
        } else {
            throw new Exception("Card not in hand.");
        }
    }
}