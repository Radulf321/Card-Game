#nullable enable

using System.Collections.Generic;
using UnityEngine;

public class CombatHandler : MonoBehaviour
{
    public static CombatHandler? instance;

    private int currentTurn = 0;
    private List<Turn> turns = new List<Turn>();
    private Dictionary<string, int> goals = new Dictionary<string, int>();
    private Dictionary<string, int> goalsThisTurn = new Dictionary<string, int>();

    private Dictionary<string, int> cardsPlayed = new Dictionary<string, int>();

    private int maxEnergy;
    private int currentEnergy = 0;

    private CardPile cardPile = new CardPile();
    private bool requireUpdate = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        CombatHandler.instance = this;
        transform.Find("BackgroundImage").GetComponent<UnityEngine.UI.Image>().sprite = Resources.Load<Sprite>(Game.Instance.GetCurrentCombatTarget().GetCombatBackgroundPath());
        CombatTarget combatTarget = Game.Instance.GetCurrentCombatTarget();
        this.maxEnergy = combatTarget.GetStartingEnergy();
        this.cardPile = combatTarget.CreateCardPile();
        this.turns = combatTarget.GenerateTurns();
        foreach (Card relic in Game.Instance.GetPlayer().GetRelics()) {
            relic.Play();
        }
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

    public int getGoalAmount(string goal)
    {
        if (goals.ContainsKey(goal))
        {
            return goals[goal];
        }
        return 0;
    }

    public int getGoalAmountThisTurn(string goal)
    {
        if (goalsThisTurn.ContainsKey(goal))
        {
            return goalsThisTurn[goal];
        }
        return 0;
    }

    public List<Turn> getTurns()
    {
        return turns;
    }

    public int getCurrentTurnIndex()
    {
        return currentTurn;
    }

    public Turn getCurrentTurn()
    {
        return getTurns()[getCurrentTurnIndex()];
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
        if (goalsThisTurn.ContainsKey(goal))
        {
            goalsThisTurn[goal] += amount;
        }
        else
        {
            goalsThisTurn.Add(goal, amount);
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
        if (!getCurrentTurn().areRequirementsFulfilled())
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
            cardPile.DrawCard(CardEffectTrigger.TurnStart);
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

    private void startTurn()
    {
        currentEnergy = maxEnergy;
        goalsThisTurn.Clear();
        Turn currentTurn = getCurrentTurn();
        foreach (CardEffect effect in currentTurn.getEffects()) {
            effect.applyEffect();
        }
        SendTriggerMessage(new TriggerMessage(TriggerType.StartTurn, new TriggerMessageData(amount: getCurrentTurnIndex())));
        updateView();
    }

    private void loose()
    {
        _ = Game.Instance.GetCurrentCombatTarget().EndCombat(false);
    }

    private void win()
    {
        _ = Game.Instance.GetCurrentCombatTarget().EndCombat(true);
    }

    private void SendTriggerMessage(TriggerMessage message)
    {
        Game.Instance.SendTriggerMessage(message);
    }
}
