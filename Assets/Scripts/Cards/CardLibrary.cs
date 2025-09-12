using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;

public class CardLibrary
{

    private readonly Dictionary<string, Card> cards;

    public CardLibrary()
    {
        this.cards = new Dictionary<string, Card>();
    }

    public CardLibrary(string resourceFolder)
    {
        Dictionary<string, Card> cards = new Dictionary<string, Card>();
        TextAsset[] jsonFiles = Resources.LoadAll<TextAsset>(resourceFolder);
        foreach (TextAsset jsonFile in jsonFiles)
        {
            JObject jsonObject = JObject.Parse(jsonFile.text);
            string cardName = jsonObject["id"].ToString();
            Card card = new Card(jsonObject);
            cards[cardName] = card;
        }
        this.cards = cards;
    }

    public Card GetCard(string cardID)
    {
        if (cards.ContainsKey(cardID))
        {
            return cards[cardID].Clone();
        }
        else
        {
            throw new System.Exception("Card not found in library: " + cardID);
        }
    }
    
    public List<Card> GetAllCards()
    {
        List<Card> allCards = new List<Card>();
        foreach (var card in cards.Values)
        {
            allCards.Add(card.Clone());
        }
        return allCards;
    }
}