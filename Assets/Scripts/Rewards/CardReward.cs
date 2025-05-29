using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using UnityEngine.Localization.Settings;

public class CardReward : Reward
{
    private Card card;

    public CardReward(Card card)
    {
        this.card = card;
    }

    public CardReward(JObject rewardData)
    {
        this.card = Game.Instance.GetCard(rewardData["card"]?.ToString() ?? "Undefined");
    }

    public Card GetCard()
    {
        return card;
    }
    public override void Collect()
    {
        Game.Instance.GetPlayer().AddCardToDeck(this.GetCard());
    }
    public override Task<string> ToNiceString()
    {
        return AsyncHelper.HandleToTask(LocalizationSettings.StringDatabase.GetLocalizedStringAsync("RewardStrings", "CardShort"));
    }
}