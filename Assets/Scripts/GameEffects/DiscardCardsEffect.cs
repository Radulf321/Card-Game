using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using UnityEngine.Localization.Settings;

public enum DiscardPosition
{
    Random,
    Left,
    Right
}

public class DiscardCardsEffect : GameEffect
{
    private int amount;
    private DiscardPosition discardPosition;
    private bool checkCardsAvailable;

    public DiscardCardsEffect(int amount, DiscardPosition discardPosition = DiscardPosition.Random, bool checkCardsAvailable = true)
    {
        this.amount = amount;
        this.discardPosition = discardPosition;
        this.checkCardsAvailable = checkCardsAvailable;
    }

    public DiscardCardsEffect(JObject json) : this(
        amount: json["amount"]?.ToObject<int>() ?? 1,
        discardPosition: EnumHelper.ParseEnum<DiscardPosition>(json["discardPosition"]?.ToString()) ?? DiscardPosition.Random,
        checkCardsAvailable: json["checkCardsAvailable"]?.ToObject<bool>() ?? true)
    {
    }

    public override void applyEffect()
    {
        // Assuming RoundHandler has a method to apply the effect
        CardPile cardPile = CombatHandler.instance.getCardPile();
        for (int i = 0; i < this.amount; i++)
        {
            List<Card> hand = cardPile.GetHand();
            if (hand.Count <= 0)
            {
                throw new Exception("No cards to discard.");
            }
            int index;
            switch (this.discardPosition)
            {
                case DiscardPosition.Random:
                    index = UnityEngine.Random.Range(0, hand.Count);
                    break;

                case DiscardPosition.Left:
                    index = 0;
                    break;

                case DiscardPosition.Right:
                    index = hand.Count - 1;
                    break;

                default:
                    throw new Exception("Invalid discard position");
            }
            cardPile.DiscardCard(hand[index], type: DiscardType.Discard);
        }
    }

    public override bool canPlay()
    {
        // The card itself is part of the hand, so there needs to be one more card in hand
        return !this.checkCardsAvailable || CombatHandler.instance.getCardPile().GetHand().Count >= (amount + 1);
    }

    public override GameEffect Clone(Card newOwner)
    {
        return new DiscardCardsEffect(this.amount);
    }

    public override Task<string> getDescription()
    {
        return AsyncHelper.HandleToTask(LocalizationSettings.StringDatabase.GetLocalizedStringAsync("CardStrings", "DiscardCards",
            arguments: new Dictionary<string, object> {
                { "amount", this.amount },
                { "discardPosition", this.discardPosition }
            }
        ));
    }
}