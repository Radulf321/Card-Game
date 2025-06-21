using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

#nullable enable

public enum CardPileType
{
    Player,
    Add,
    Replace,
}

public class CardPileFactory
{
    private List<Card> cards;
    private CardPileType type;
    private bool shuffle;
    private int startingHandSize;

    public CardPileFactory(JObject? json = null)
    {
        this.type = EnumHelper.ParseEnum<CardPileType>(json?["type"]?.ToString()) ?? CardPileType.Player;
        this.shuffle = json?["shuffle"]?.ToObject<bool>() ?? true;
        this.startingHandSize = json?["startingHandSize"]?.ToObject<int>() ?? 5;
        List<Card> cards = new List<Card>();
        foreach (JToken cardID in json?["cards"] ?? new JArray())
        {
            cards.Add(Game.Instance.GetCard(cardID.ToString()));
        }
        this.cards = cards;
    }

    public CardPile CreateCardPile()
    {
        List<Card> deck;
        switch (this.type)
        {
            case CardPileType.Player:
                deck = Game.Instance.GetPlayer().GetDeck();
                break;
            case CardPileType.Add:
                deck = new List<Card>(Game.Instance.GetPlayer().GetDeck());
                deck.AddRange(this.cards);
                break;
            case CardPileType.Replace:
                deck = new List<Card>(this.cards);
                break;
            default:
                throw new Exception("Unknown CardPileType: " + this.type);
        }
        CardPile cardPile = new CardPile(deck);
        if (this.shuffle)
        {
            cardPile.ShuffleDeck();
        }
        cardPile.DrawCards(this.startingHandSize, CardEffectTrigger.CombatStart);
        return cardPile;
    }
}