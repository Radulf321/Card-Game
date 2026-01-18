using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
#nullable enable

public enum CardAfterPlay
{
    Discard,
    RemoveCurrentGame,
    RemovePermanent,
    StayInPlay
}

public enum CardType
{
    Regular,
    Relic,
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

public class Card : IClonable<Card>
{
    private CardType type;
    private int? cost;
    private List<GameEffect> effects;
    private string name;
    private string imagePath;
    private string id;
    private CardAfterPlay afterPlay = CardAfterPlay.Discard;

    public Card(int? cost, string id, List<GameEffect> effects, string name, string imagePath, CardType type = CardType.Regular)
    {
        this.cost = cost;
        this.effects = effects;
        this.name = name;
        this.imagePath = imagePath;
        this.id = id;
        this.type = type;
    }

    public Card(JObject cardData)
    {
        this.cost = cardData["cost"]?.ToObject<int>();
        this.name = LocalizationHelper.GetLocalizedString(cardData["name"] as JObject)!;
        this.imagePath = cardData["image"]?.ToString() ?? "Placeholder";
        this.id = cardData["id"]!.ToString();
        List<GameEffect> effects = new List<GameEffect>();
        foreach (JObject effectData in cardData["effects"] ?? new JArray())
        {
            effects.Add(GameEffect.FromJson(effectData, this));
        }
        this.effects = effects;
        this.type = EnumHelper.ParseEnum<CardType>(cardData["type"]?.ToString()) ?? CardType.Regular;
    }

    public bool CanPlay(Enemy? target = null)
    {
        foreach (GameEffect effect in effects)
        {
            if (!effect.canPlay(target))
            {
                return false;
            }
        }
        return (this.cost == null) || (this.cost <= CombatHandler.instance?.getCurrentEnergy());
    }

    public void Play(bool force = false, Enemy? target = null)
    {
        CombatHandler? combatHandler = CombatHandler.instance;
        if (combatHandler == null)
        {
            throw new Exception("Cannot play card without combat handler");
        }
        if (CanPlay() || force)
        {
            this.afterPlay = CardAfterPlay.Discard;
            CardPile cardPile = combatHandler.getCardPile();
            cardPile.AddCardToPlay(this);
            if ((this.cost != null) && !force)
            {
                combatHandler.looseEnergy(this.cost.Value);
            }
            foreach (GameEffect effect in effects)
            {
                effect.applyEffect(target);
            }
            combatHandler.playCard(this.id);
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

            Game.Instance.SendTriggerMessage(new TriggerMessage(TriggerType.PlayCard, new TriggerMessageData(card: this)));
        }
        else
        {
            throw new System.Exception("Not enough energy to play this card.");
        }
    }

    public int? GetCost()
    {
        return this.cost;
    }

    public CardType GetCardType()
    {
        return this.type;
    }

    public string GetName()
    {
        return this.name;
    }

    public async Task<string> GetTextDescription()
    {
        List<string> descriptions = new List<string>();
        foreach (GameEffect effect in this.effects)
        {
            descriptions.Add(await effect.getDescription());
        }
        return string.Join("\n", descriptions);
    }

    public async Task<string> GetDescription()
    {
        List<string> descriptions = new List<string>();
        foreach (GameEffect effect in this.effects)
        {
            descriptions.Add(await effect.GetIconDescription());
        }
        return string.Join("\n", descriptions);
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

    public void SetEffects(List<GameEffect> effects)
    {
        this.effects = effects;
    }

    public Card Clone()
    {
        Card clone = new Card(
            cost: this.cost,
            id: this.id,
            effects: new List<GameEffect>(),
            name: this.name,
            imagePath: this.imagePath,
            type: this.GetCardType()
        );
        List<GameEffect> clonedEffects = this.effects.Select(effect => effect.Clone(clone)).ToList();
        clone.SetEffects(clonedEffects);
        return clone;
    }

    public void OnDraw()
    {
        foreach (GameEffect effect in this.effects) {
            effect.OnDraw();
        }
    }
}
