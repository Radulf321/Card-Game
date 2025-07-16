using UnityEngine;
using UnityEngine.UI;
using TMPro;

#nullable enable
public class RewardDisplayHandler : MonoBehaviour, IViewUpdater
{
    private Reward? reward;
    private bool shouldUpdate = false;
    private Color? textColor;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (this.shouldUpdate)
        {
            if (reward is CardReward)
            {
                CardReward cardReward = (reward as CardReward)!;
                transform.Find("SpriteReward").gameObject.SetActive(false);
                Transform cardTransform = transform.Find("CardReward");
                cardTransform.gameObject.SetActive(true);
                CardHandler cardHandler = cardTransform.GetComponentInChildren<CardHandler>();
                cardHandler.SetCard(cardReward.GetCard());
                cardHandler.SetActive(false);
            }
            else if (reward != null)
            {
                transform.Find("CardReward").gameObject.SetActive(false);
                Transform spriteTransform = transform.Find("SpriteReward");
                spriteTransform.Find("Image").GetComponent<Image>().sprite = reward.GetSprite();
                TextMeshProUGUI text = spriteTransform.Find("Text").GetComponent<TextMeshProUGUI>();
                AsyncHelper.UpdateTextFromTask(text, reward.GetCaption());
                if (this.textColor != null)
                {
                    text.color = this.textColor.Value;
                }
                spriteTransform.gameObject.SetActive(true);

            }
            transform.GetComponentInParent<LayoutElement>().preferredWidth = transform.GetComponentInParent<RectTransform>().rect.height * transform.GetComponentInParent<AspectRatioFitter>().aspectRatio;
            this.shouldUpdate = false;
        }
    }

    protected virtual void OnRectTransformDimensionsChange()
    {
        this.shouldUpdate = true;
    }

    public void SetReward(Reward reward)
    {
        this.reward = reward;
        updateView();
    }

    public void SetTextColor(Color? color)
    {
        this.textColor = color;
        updateView();
    }

    public Reward? GetReward()
    {
        return this.reward;
    }

    public void updateView()
    {
        this.shouldUpdate = true;
    }
}
