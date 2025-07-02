using System.Collections.Generic;
using Newtonsoft.Json.Linq;

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
}