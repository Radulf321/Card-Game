using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

#nullable enable
public class RewardAreaHandler : MonoBehaviour, IViewUpdater
{
    public GameObject? rewardPrefab;
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

    public void updateView()
    {
        this.updateChildrenViews<RewardAreaHandler, RewardDisplayHandler, Reward>(
            this.rewards,
            (Reward reward) =>
            {
                GameObject rewardObject = Instantiate(rewardPrefab!);
                RewardDisplayHandler rewardHandler = rewardObject.GetComponent<RewardDisplayHandler>();
                rewardHandler.SetTextColor(this.textColor);
                rewardHandler.SetReward(reward);
                LayoutElement myLayout = transform.GetComponent<LayoutElement>();
                LayoutElement rewardLayout = rewardObject.GetComponent<LayoutElement>();
                rewardLayout.preferredHeight = myLayout.preferredHeight;
                rewardLayout.flexibleHeight = myLayout.flexibleHeight;
                return rewardObject;
            },
            (RewardDisplayHandler rewardHandler) =>
            {
                return rewardHandler.GetReward()!;
            }
        );

        if (rewards.Count > 1)
        {
            float rewardWidth = GetComponent<RectTransform>().rect.height * rewardPrefab!.GetComponent<AspectRatioFitter>().aspectRatio;
            float myWidth = GetComponent<RectTransform>().rect.width;

            float totalCardWidth = rewardWidth * rewards.Count;
            float totalSpacing = myWidth - totalCardWidth;
            float spacingPerCard = Mathf.Min(totalSpacing / (rewards.Count - 1), myWidth * 0.02f);

            transform.GetComponent<HorizontalLayoutGroup>().spacing = spacingPerCard;
        }
    }
}
