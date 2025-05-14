using System;
using System.Collections.Generic;

#nullable enable
public class BasedOnPreviousPlaysEffect : CardEffect {
    private Func<int, List<CardEffect>> generateEffects;
    private string? descriptionSuffix;
    private string cardType;

    public BasedOnPreviousPlaysEffect(Func<int, List<CardEffect>> generateEffects, string? descriptionSuffix, string cardType) {
        this.cardType = cardType;
        this.generateEffects = generateEffects;
        this.descriptionSuffix = descriptionSuffix;
    }

    public override void applyEffect() {
        int previousPlayed = CombatHandler.instance.getCardsPlayed(cardType);
        foreach (CardEffect effect in generateEffects(previousPlayed)) {
            effect.applyEffect();
        }
    }

    public override string getDescription() {
        List<string> descriptions = new List<string>();
        foreach (CardEffect effect in generateEffects(CombatHandler.instance.getCardsPlayed(cardType))) {
            descriptions.Add(effect.getDescription());
        }
        if (descriptionSuffix != null) {
            descriptions.Add(descriptionSuffix);
        }
        return string.Join("\n", descriptions);
    }
}