using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

#nullable enable
public class CardsEquipmentEffect : EquipmentEffect
{
    private List<string> cards;
    public CardsEquipmentEffect(JObject effectData)
    {
        this.cards = effectData["cards"]?.ToObject<List<string>>() ?? new List<string>();
    }

    public override void ApplyEffect(Player player)
    {
        foreach (string cardID in cards)
        {
            player.AddCardToDeck(Game.Instance.GetCard(cardID));
        }
    }

    public override Task<string> GetCaption()
    {
        // Card Equipment does not have a caption as it is displayed explicitly in the UI
        throw new System.NotImplementedException();
    }

    public List<Card> GetCards()
    {
        List<Card> cardList = new List<Card>();
        foreach (string cardID in cards)
        {
            Card? card = Game.Instance.GetCard(cardID);
            if (card != null)
            {
                cardList.Add(card);
            }
        }
        return cardList;
    }
}