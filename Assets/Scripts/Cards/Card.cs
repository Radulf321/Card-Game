using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
#nullable enable

public enum CardAfterPlay {
    Discard,
    RemoveCurrentGame,
    RemovePermanent,
    StayInPlay
}

public static class CardAfterPlayHelper
{
    public static CardAfterPlay FromJson(JToken json)
    {
        if (Enum.TryParse<CardAfterPlay>(json.ToString(), true, out var result))
        {
            return result;
        }
        else
        {
            throw new ArgumentException($"Invalid CardAfterPlay value: {json.ToString()}");
        }
    }
}

public class Card
{
    private int cost;
    private List<CardEffect> effects;
    private string name;
    private string imagePath;
    private string id;
    private CardAfterPlay afterPlay = CardAfterPlay.Discard;

    public Card(int cost, string id, List<CardEffect> effects, string name, string imagePath)
    {
        this.cost = cost;
        this.effects = effects;
        this.name = name;
        this.imagePath = imagePath;
        this.id = id;
    }

    public Card(int cost, string id, CardEffect effect, string name, string imagePath)
        : this(cost, id, new List<CardEffect> { effect }, name, imagePath)
    {
    }

    public Card(JObject cardData)
    {
        this.cost = cardData["cost"]?.ToObject<int>() ?? 0;
        this.name = LocalizationHelper.GetLocalizedString(cardData["name"] as JObject);
        this.imagePath = cardData["image"]?.ToString() ?? "Placeholder";
        this.id = cardData["id"]!.ToString();
        List<CardEffect> effects = new List<CardEffect>();
        foreach (JObject effectData in cardData["effects"] ?? new JArray())
        {
            effects.Add(CardEffect.FromJson(effectData, this));
        }
        this.effects = effects;
    }

    public bool CanPlay()
    {
        return this.cost <= CombatHandler.instance.getCurrentEnergy();
    }

    public void Play()
    {
        if (CanPlay())
        {
            this.afterPlay = CardAfterPlay.Discard;
            CardPile cardPile = CombatHandler.instance.getCardPile();
            cardPile.AddCardToPlay(this);
            CombatHandler.instance.looseEnergy(this.cost);
            foreach (CardEffect effect in effects)
            {
                effect.applyEffect();
            }
            CombatHandler.instance.playCard(this.id);
            switch (this.afterPlay)
            {
                case CardAfterPlay.Discard:
                    cardPile.DiscardCard(this);
                    break;

                case CardAfterPlay.RemoveCurrentGame:
                    cardPile.RemoveCard(this);
                    break;

                case CardAfterPlay.RemovePermanent:
                    cardPile.RemoveCard(this);
                    Game.Instance.GetPlayer().RemoveCardFromDeck(this);
                    break;

                case CardAfterPlay.StayInPlay:
                    // Nothing to do, card is in play and stays there
                    break;

                default:
                    throw new Exception("Unknown After Play state");
            }
        }
        else
        {
            throw new System.Exception("Not enough energy to play this card.");
        }
    }

    public int GetCost()
    {
        return this.cost;
    }

    public string GetName()
    {
        return this.name;
    }

    public string GetDescription()
    {
        return string.Join("\n", effects.Select(effect => effect.getDescription()));
    }

    public string GetImagePath()
    {
        return Game.Instance.GetResourcePath() + "/Graphics/Cards/" + this.imagePath;
    }

    public string GetID()
    {
        return this.id;
    }

    public void SetAfterPlay(CardAfterPlay afterPlay)
    {
        this.afterPlay = afterPlay;
    }

    public void SetEffects(List<CardEffect> effects)
    {
        this.effects = effects;
    }

    public Card Clone()
    {
        Card clone = new Card(cost: this.cost, id: this.id, effects: new List<CardEffect>(), name: this.name, imagePath: this.imagePath);
        List<CardEffect> clonedEffects = this.effects.Select(effect => effect.Clone(clone)).ToList();
        clone.SetEffects(clonedEffects);
        return clone;
    }
}
