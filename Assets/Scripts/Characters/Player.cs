using System.Collections.Generic;
using Newtonsoft.Json.Linq;

public class Player
{
    private List<Card> deck;
    private List<Card> relics;
    private List<Skill> skills;
    private EnergyInfo energyInfo;
    private Dictionary<string, int> currencies;

    public Player()
    {
        this.deck = new List<Card>();
        this.relics = new List<Card>();
        this.energyInfo = new EnergyInfo();
        this.currencies = new Dictionary<string, int>();
        this.skills = new List<Skill>();
    }

    public Player(List<string> startingCards, JObject energyInfo)
    {
        List<Card> deck = new List<Card>();
        foreach (string cardId in startingCards)
        {
            deck.Add(Game.Instance.GetCard(cardId).Clone());
        }
        this.deck = deck;
        this.relics = new List<Card>();
        this.energyInfo = new EnergyInfo(energyInfo);
        this.currencies = new Dictionary<string, int>();
    }

    public Player(JObject json)
    {
        this.deck = new List<Card>();
        this.relics = new List<Card>();
        this.energyInfo = new EnergyInfo();
        this.currencies = new Dictionary<string, int>();
        this.skills = new List<Skill>();

        // Load deck from data
        if (json["deck"] != null)
        {
            foreach (string cardId in json["deck"]!.ToObject<List<string>>()!)
            {
                AddCardToDeck(Game.Instance.GetCard(cardId));
            }
        }

        // Load relics from data
        if (json["relics"] != null)
        {
            foreach (string cardId in json["relics"]!.ToObject<List<string>>()!)
            {
                AddCardToDeck(Game.Instance.GetCard(cardId));
            }
        }

        // Load energy info from data
        if (json["energyInfo"] != null)
        {
            this.energyInfo = new EnergyInfo(json["energyInfo"]! as JObject);
        }

        // Load currencies from data
        if (json["currencies"] != null)
        {
            this.currencies = json["currencies"]!.ToObject<Dictionary<string, int>>() ?? new Dictionary<string, int>();
        }

        if (json["skills"] != null)
        {
            foreach (JObject skillJson in json["skills"]!.ToObject<List<JObject>>()!)
            {
                Skill skill = Game.Instance.GetSkill(skillJson["id"]!.ToString());
                skill.AddProgress(skillJson["progress"]!.ToObject<int>());
                this.skills.Add(skill);
            }
        }
    }

    public List<Card> GetDeck()
    {
        return deck;
    }

    public List<Card> GetRelics()
    {
        return relics;
    }

    public void AddCardToDeck(Card card)
    {
        Card clonedCard = card.Clone();
        switch (clonedCard.GetCardType())
        {
            case CardType.Regular:
                this.deck.Add(clonedCard);
                break;

            case CardType.Relic:
                this.relics.Add(clonedCard);
                break;
        }
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

    public int GetCurrency(string currencyID)
    {
        if (this.currencies.ContainsKey(currencyID))
        {
            return this.currencies[currencyID];
        }
        return 0;
    }

    public void AddCurrency(string currencyID, int amount)
    {
        if (this.currencies.ContainsKey(currencyID))
        {
            this.currencies[currencyID] += amount;
        }
        else
        {
            this.currencies[currencyID] = amount;
        }
        Game.Instance.SendTriggerMessage(
            new TriggerMessage(
                TriggerType.AddCurrency,
                new TriggerMessageData(amount: amount, currency: currencyID)
            )
        );
    }

    public JObject ToJson()
    {
        JObject saveData = new JObject();
        JArray deckArray = new JArray();
        foreach (Card card in this.deck)
        {
            deckArray.Add(card.GetID());
        }
        saveData["deck"] = deckArray;
        JArray relicsArray = new JArray();
        foreach (Card card in this.relics)
        {
            relicsArray.Add(card.GetID());
        }
        saveData["relics"] = relicsArray;
        saveData["energyInfo"] = this.energyInfo.SaveToJson();
        JObject currenciesObject = new JObject();
        foreach (KeyValuePair<string, int> currency in this.currencies)
        {
            currenciesObject[currency.Key] = currency.Value;
        }
        saveData["currencies"] = currenciesObject;
        JArray skillsArray = new JArray();
        foreach (Skill skill in this.skills)
        {
            skillsArray.Add(
                new JObject()
                {
                    ["id"] = skill.GetID(),
                    ["progress"] = skill.GetProgress(),
                }
            );
        }
        saveData["skills"] = skillsArray;
        return saveData;
    }
}