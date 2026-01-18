#nullable enable

using System.Collections.Generic;
using UnityEngine;

public class CombatHandler : MonoBehaviour
{
    public static CombatHandler? instance;

    private int currentTurn = 0;
    private List<Turn>? turns = new List<Turn>();
    private List<Enemy>? enemies = new List<Enemy>();
    private GoalManager? goalManager;

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
        this.enemies = combatTarget.GenerateEnemies();
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
            transform.GetComponentInChildren<EnemyAreaHandler>().updateView();
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
        return this.GetGoalManager().GetGoalAmount(goal);
    }

    public int getGoalAmountThisTurn(string goal)
    {
        return this.GetGoalManager().GetGoalAmountThisTurn(goal);
    }

    public List<Turn>? getTurns()
    {
        return turns;
    }

    public int getCurrentTurnIndex()
    {
        return currentTurn;
    }

    public Turn? getCurrentTurn()
    {
        return getTurns()?[getCurrentTurnIndex()];
    }

    public List<Enemy>? getEnemies()
    {
        return enemies;
    }

    public int getTotal()
    {
        return this.GetGoalManager().GetTotal();
    }

    public void addGoal(string goal, int amount, CardEffectTrigger? trigger = null)
    {
        this.GetGoalManager().AddGoal(goal, amount, trigger);
    }

    public void endTurn()
    {
        SendTriggerMessage(new TriggerMessage(TriggerType.EndTurn, new TriggerMessageData(amount: getCurrentTurnIndex())));
        // Conditions not fulfilled -> Lost
        if (!getCurrentTurn()?.areRequirementsFulfilled() ?? false)
        {
            loose();
        }
        // It was the last turn -> Win
        else if ((currentTurn >= ((turns?.Count ?? 0) - 1)) && ((enemies?.Count ?? 0) == 0))
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

    public void UseSkill()
    {
        Skill skillToUse = Game.Instance.GetPlayer().GetSkills()[0];
        if (skillToUse.CanUse())
        {
            skillToUse.Use();
        }
        else
        {
            UnityEngine.Debug.Log("Not enough Progress to use the skill.");
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

    public GoalManager GetGoalManager()
    {
        if (this.goalManager == null)
        {
            this.goalManager = new GoalManager();
        }
        return this.goalManager;
    }

    public void RemoveEnemy(Enemy enemy)
    {
        this.enemies?.Remove(enemy);
        updateView();
    }

    private void startTurn()
    {
        currentEnergy = maxEnergy;
        this.GetGoalManager().ResetGoalsThisTurn();
        foreach (Enemy enemy in enemies ?? new List<Enemy>()) {
            enemy.GetGoalManager().ResetGoalsThisTurn();
        }
        Turn? currentTurn = getCurrentTurn();
        foreach (GameEffect effect in currentTurn?.getEffects() ?? new List<GameEffect>()) {
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
