using System.Collections.Generic;
using UnityEngine;

public class DialogCardRewardAreaHandler : CardsContainerHandler<Reward>
{
    private List<Reward> rewards;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {

    }


    public void SetRewards(List<Reward> rewards)
    {
        this.rewards = rewards;
        updateView();
    }

    protected override List<Reward> GetCardData()
    {
        return this.rewards;
    }

    protected override Reward GetHandlerData(CardHandler cardHandler)
    {
        // For now, there is only the CardReward, so we'll handle it accordingly
        // We'll need to adjust once we get more reward types
        Card card = cardHandler.GetCard();
        return this.rewards.Find(reward => ((reward is CardReward) && ((reward as CardReward).GetCard() == card)));
    }

    protected override void SetHandlerData(CardHandler cardHandler, Reward cardData)
    {
        if (cardData is CardReward)
        {
            cardHandler.SetCard((cardData as CardReward).GetCard());
        }
        cardHandler.SetActive(false);
    }
}
