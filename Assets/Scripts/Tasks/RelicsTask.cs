using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Unity.Mathematics;
using UnityEngine.Localization.Settings;

#nullable enable
public class RelicsTask : GameTask
{
    private int amount;

    public RelicsTask(JObject json) : base(json)
    {
        this.amount = json["amount"]?.ToObject<int>() ?? 1;
    }

    public override async Task<string> GetDescription()
    {
        string description = await AsyncHelper.HandleToTask(LocalizationSettings.StringDatabase.GetLocalizedStringAsync("TaskStrings", "RelicsDescription",
            arguments: new Dictionary<string, object?> {
                { "amount", this.amount }
            }
        ));
        return description;
    }

    public override int? GetProgress()
    {
        List<Card> deck = Game.Instance.GetPlayer().GetDeck();
        int relicCount = 0;
        foreach (Card card in deck)
        {
            if (card.GetCardType() == CardType.Relic)
            {
                relicCount++;
            }
        }
        return math.min(this.amount, relicCount);
    }

    public override int? GetTotal()
    {
        return this.amount;
    }
}