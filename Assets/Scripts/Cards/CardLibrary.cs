using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;

public enum CardType {
    Flirt,
    Chat,
    SweetTalk,
    PlanAhead,
    Compliment,
    EnergyBurst,
    RiskyPlay,
}

public class CardLibrary {
    public static Card getCard(CardType cardType) {
        switch (cardType) {
            case CardType.Flirt:
                return new Card(
                    1,
                    cardType,
                    new List<CardEffect> {
                        new GoalCardEffect(Goal.SmallTalk, 1),
                        new GoalCardEffect(Goal.Lust, 1)
                    },
                    "Flirt",
                    null,
                    "Placeholder"
                );
            case CardType.Chat:
                return new Card(
                    1,
                    cardType,
                    new GoalCardEffect(Goal.SmallTalk, 2),
                    "Chat",
                    null,
                    "Placeholder"
                );
            case CardType.SweetTalk:
                return new Card(
                    2,
                    cardType,
                    new List<CardEffect> {
                        new GoalCardEffect(Goal.Lust, 2),
                        new DrawCardsEffect(1)
                    },
                    "Sweet Talk",
                    null, 
                    "Placeholder"
                );
            case CardType.PlanAhead:
                return new Card(
                    2,
                    cardType,
                    new DrawCardsEffect(2),
                    "Plan Ahead",
                    null,
                    "Placeholder"
                );
            case CardType.Compliment:
                return new Card(
                    2,
                    cardType,
                    new BasedOnPreviousPlaysEffect(
                        (int previousPlayed) => new List<CardEffect>() {
                            new GoalCardEffect(Goal.Lust, Math.Max(0, 3 - previousPlayed)),
                            new GoalCardEffect(Goal.SmallTalk, Math.Max(0, 3 - previousPlayed)),
                        },
                        "Decrease the effect by 1 for each Compliment played",
                        cardType.ToString()
                    ),
                    "Compliment",
                    null,
                    "Placeholder"
                );
            case CardType.EnergyBurst:
                return new Card(
                    0,
                    cardType,
                    new EnergyEffect(1),
                    "Energy Burst",
                    null,
                    "Placeholder"
                );
            case CardType.RiskyPlay:
                return new Card(
                    1,
                    cardType,
                    new List<CardEffect> {
                        new DiscardCardsEffect(1),
                        new GoalCardEffect(Goal.SmallTalk, 4)
                    },
                    "Risky Play",
                    null,
                    "Placeholder"
                );
                // IDEA: Increase maximum energy
                // IDEA: Weak effect, but gets stronger for each time played
            default:
                throw new System.Exception("Card type not recognized.");
        }
    }

    private readonly Dictionary<string, Card> cards;

    public CardLibrary() {
        this.cards = new Dictionary<string, Card>();
    }

    public CardLibrary(string resourceFolder) {
        Dictionary<string, Card> cards = new Dictionary<string, Card>();
        TextAsset[] jsonFiles = Resources.LoadAll<TextAsset>(resourceFolder);
        foreach (TextAsset jsonFile in jsonFiles) {
            JObject jsonObject = JObject.Parse(jsonFile.text);
            string cardName = jsonObject["id"].ToString();
            Card card = new Card(jsonObject);
            cards[cardName] = card;
        }
        this.cards = cards;
    }

    public Card GetCard(string cardID) {
        if (cards.ContainsKey(cardID)) {
            return cards[cardID].Clone();
        } else {
            throw new System.Exception("Card not found in library.");
        }
    }
}