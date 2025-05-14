using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using UnityEngine;
#nullable enable

public class Card
{
    private int cost;
    private List<CardEffect> effects;
    private string name;
    private string imagePath;
    private string id;

    public Card(int cost, CardType cardType, List<CardEffect> effects, string name, Func<string>? generateDescription, string imagePath) {
        this.cost = cost;
        this.effects = effects;
        this.name = name;
        this.imagePath = imagePath;
        this.id = cardType.ToString();
    }

    public Card(int cost, CardType cardType, CardEffect effect, string name, Func<string>? generateDescription, string imagePath)
        : this(cost, cardType, new List<CardEffect> { effect }, name, generateDescription, imagePath) {
    }

    public Card(JObject cardData) {
        this.cost = cardData["cost"]?.ToObject<int>() ?? 0;
        this.name = LocalizationHelper.GetLocalizedString(cardData["name"] as JObject);
        this.imagePath = cardData["image"]?.ToString() ?? "Placeholder";
        this.id = cardData["id"]!.ToString();
        List<CardEffect> effects = new List<CardEffect>();
        foreach (JObject effectData in cardData["effects"] ?? new JArray()) {
            effects.Add(CardEffect.FromJson(effectData, this));
        }
        this.effects = effects;
    }

    public bool CanPlay() {
        return this.cost <= CombatHandler.instance.getCurrentEnergy();
    }

    public void Play() {
        if (CanPlay()) {
            CombatHandler.instance.getCardPile().DiscardCard(this);
            CombatHandler.instance.looseEnergy(this.cost);
            foreach (CardEffect effect in effects) {
                effect.applyEffect();
            }
            CombatHandler.instance.playCard(this.id);
        } else {
            throw new System.Exception("Not enough energy to play this card.");
        }
    }

    public int GetCost() {
        return this.cost;
    }

    public string GetName() {
        return this.name;
    }

    public string GetDescription() {
        return string.Join("\n", effects.Select(effect => effect.getDescription()));
    }

    public string GetImagePath() {
        return Game.Instance.GetResourcePath() + "/Graphics/Cards/" + this.imagePath;
    }

    public string GetID() {
        return this.id;
    }
}
