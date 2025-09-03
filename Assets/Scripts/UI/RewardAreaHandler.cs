using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

#nullable enable
public class RewardAreaHandler : GenericCardsContainerHandler<Reward, RewardDisplayHandler>, IViewUpdater
{
    public Color textColor = Color.white;
    List<Reward> rewards = new List<Reward>();

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

    protected override void SetHandlerData(RewardDisplayHandler handler, Reward cardData)
    {
        handler.SetTextColor(this.textColor);
        handler.SetReward(cardData);
    }

    protected override Reward? GetHandlerData(RewardDisplayHandler handler)
    {
        return handler.GetReward();
    }

    protected override RewardDisplayHandler GetHandler(Transform transform)
    {
        return transform.GetComponent<RewardDisplayHandler>();
    }
}
