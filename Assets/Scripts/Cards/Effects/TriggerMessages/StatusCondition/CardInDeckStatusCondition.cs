using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;

public class CardInDeckStatusCondition : StatusCondition
{
    private string cardID;

    public CardInDeckStatusCondition(JObject json)
    {
        this.cardID = json["card"]!.ToString();
    }

    public override bool IsFulfilled()
    {
        Player player = Game.Instance.GetPlayer();
        foreach (List<Card> cards in new List<List<Card>>() { player.GetDeck(), player.GetRelics() })
        {
            if (cards.Any(card => card.GetID() == this.cardID))
            {
                return true;
            }
        }
        return false;
    }
}