using System;
using System.Collections.Generic;
using UnityEngine;

public class CombatHandler : MonoBehaviour
{
    public static CombatHandler instance;

    private int currentTurn = 0;
    private List<Turn> turns;
    private Dictionary<string, int> goals = new Dictionary<string, int>();

    private Dictionary<string, int> cardsPlayed = new Dictionary<string, int>();

    private int maxEnergy = 2;
    private int currentEnergy = 0;

    private CardPile cardPile;
    private bool requireUpdate = false;
    private List<Action<TriggerMessage>> triggerMessageSubscribers = new List<Action<TriggerMessage>>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        CombatHandler.instance = this;
        transform.Find("BackgroundImage").GetComponent<UnityEngine.UI.Image>().sprite = Resources.Load<Sprite>(Game.Instance.GetCurrentCombatTarget().GetCombatBackgroundPath());
        this.cardPile = new CardPile(Game.Instance.GetPlayer().GetDeck());
        this.turns = Game.Instance.GetCurrentCombatTarget().GenerateTurns();
        cardPile.ShuffleDeck();
        cardPile.DrawCards(5);
        startTurn();
    }

    // Update is called once per frame
    void Update()
    {
        if (this.requireUpdate)
        {
            this.requireUpdate = false;
            transform.GetComponentInChildren<TurnsAreaHandler>().updateView();
            transform.GetComponentInChildren<EnergyAreaHandler>().updateView();
            transform.GetComponentInChildren<CardsAreaHandler>().updateView();
            transform.GetComponentInChildren<CardsInPlayAreaHandler>().updateView();
        }
    }

    public int getCurrentEnergy()
    {
        return currentEnergy;
    }

    public int getMaxEnergy()
    {
        return maxEnergy;
    }

    public void gainEnergy(int amount, bool maxEnergy = false)
    {
        if (maxEnergy)
        {
            this.maxEnergy += amount;
        }
        this.currentEnergy += amount;
        updateView();
    }

    public void looseEnergy(int amount)
    {
        this.currentEnergy -= amount;
        updateView();
    }

    public int getSubjectAmount(string goal)
    {
        if (goals.ContainsKey(goal))
        {
            return goals[goal];
        }
        return 0;
    }

    public List<Turn> getTurns()
    {
        return turns;
    }

    public int getCurrentTurn()
    {
        return currentTurn;
    }

    public int getTotal()
    {
        int total = 0;
        foreach (KeyValuePair<string, int> goal in goals)
        {
            total += goal.Value;
        }
        return total;
    }

    public void addGoal(string goal, int amount, CardEffectTrigger? trigger = null)
    {
        if (goals.ContainsKey(goal))
        {
            goals[goal] += amount;
        }
        else
        {
            goals.Add(goal, amount);
        }
        if (trigger == CardEffectTrigger.PlayCard)
        {
            this.SendTriggerMessage(new TriggerMessage(TriggerType.AddGoal, new TriggerMessageData(amount: amount, goal: goal)));
        }
        updateView();
    }

    public void endTurn()
    {
        // Conditions not fulfilled -> Lost
        if (!turns[currentTurn].areRequirementsFulfilled())
        {
            loose();
        }
        // It was the last turn -> Win
        else if (currentTurn >= (turns.Count - 1))
        {
            win();
        }
        // Otherwise: Next turn
        else
        {
            // TODO: Some start of turn effect, e.g., increase max energy, draw cards, add complication, ...
            currentTurn++;
            cardPile.DrawCard();
            startTurn();
        }
    }

    public CardPile getCardPile()
    {
        return this.cardPile;
    }

    public void playCard(string cardID)
    {
        // Card cares about most things by itself, just add it to the played cards
        if (cardsPlayed.ContainsKey(cardID))
        {
            cardsPlayed[cardID]++;
        }
        else
        {
            cardsPlayed.Add(cardID, 1);
        }
    }

    public int getCardsPlayed(string cardID)
    {
        if (cardsPlayed.ContainsKey(cardID))
        {
            return cardsPlayed[cardID];
        }
        else
        {
            return 0;
        }
    }

    public void updateView()
    {
        this.requireUpdate = true;
    }

    public void SubscribeToTriggerMessages(Action<TriggerMessage> subscriber)
    {
        this.triggerMessageSubscribers.Add(subscriber);
    }

    public void UnsubscribeFromTriggerMessages(Action<TriggerMessage> subscriber)
    {
        this.triggerMessageSubscribers.Remove(subscriber);
    }

    private void startTurn()
    {
        currentEnergy = maxEnergy;
        SendTriggerMessage(new TriggerMessage(TriggerType.StartTurn));
        updateView();
    }

    private void loose()
    {
        Game.Instance.GetCurrentCombatTarget().EndCombat(false);
    }

    private void win()
    {
        Game.Instance.GetCurrentCombatTarget().EndCombat(true);
    }

    private void SendTriggerMessage(TriggerMessage message)
    {
        foreach (Action<TriggerMessage> subscriber in this.triggerMessageSubscribers)
        {
            subscriber(message);
        }
    }
}
