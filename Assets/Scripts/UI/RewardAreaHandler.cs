using UnityEngine;
using System.Collections.Generic;

#nullable enable
public class RewardAreaHandler : GenericCardsContainerHandler<Reward, RewardDisplayHandler>, IViewUpdater
{
    public Color textColor = Color.white;
    private float? maximumFontSize;
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

    public float? GetMaximumFontSize()
    {
        return this.maximumFontSize;
    }

    public void SetMaximumFontSize(float? fontSize)
    {
        this.maximumFontSize = fontSize;
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
