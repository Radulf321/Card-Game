using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

#nullable enable

public class DialogOptionCardHandler : MonoBehaviour, IViewUpdater, IScalable
{
    private static float optionHeight = 1238f;
    private static float optionWidth = 778f;
    private static float cardHeight = 1038f;
    private static float costHeight = 180f;
    private static float costFontSize = 96f;

    private bool needTextUpdate = false;
    private DialogOption? option;
    private Action? onClick;
    private string? costText;
    private float? scaledHeight;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (this.needTextUpdate)
        {
            UpdateText();
            needTextUpdate = false;
        }
    }

    public void SetOption(DialogOption option)
    {
        this.option = option;
        CardHandler cardHandler = transform.Find("Card").GetComponentInChildren<CardHandler>();
        Card? card = option.GetCard();
        if (card != null)
        {
            cardHandler.SetCard(card);
        }
        else
        {
            cardHandler.SetCostSprite(null);
            cardHandler.SetDescriptionFontSize(36f);
        }
        string? title = option.GetTitle();
        if (title != null)
        {
            cardHandler.SetTitle(title);
        }
        string? description = option.GetDescription();
        if (description != null)
        {
            cardHandler.SetDescription(description);
        }
        string? imagePath = option.GetImagePath();
        if (imagePath != null)
        {
            cardHandler.SetSprite(Resources.Load<Sprite>(imagePath));
        }
        cardHandler.SetOnClickAction(() =>
        {
            option.Select(this.onClick);
        });

        option.GetCostText().ContinueWith(task =>
        {
            this.costText = task.Result;
            this.needTextUpdate = true;
        }, TaskScheduler.FromCurrentSynchronizationContext());

        this.needTextUpdate = true;
    }

    public DialogOption? GetOption()
    {
        return this.option;
    }

    public void SetOnClick(Action onClick)
    {
        this.onClick = onClick;
    }

    public void SetScale(float scale)
    {
        transform.GetComponent<RectTransform>().sizeDelta = new Vector2(optionWidth * scale, GetReferenceHeight() * scale);
        transform.Find("Card").GetComponentInChildren<CardHandler>().SetScale(scale);
        if (this.costText != null)
        {
            Transform costText = transform.Find("CostText");
            costText.GetComponent<RectTransform>().sizeDelta = new Vector2(optionWidth * scale, costHeight * scale);
            costText.GetComponent<TMPro.TextMeshProUGUI>().fontSize = costFontSize * scale;
        }
    }

    public void SetHeight(float height)
    {
        this.scaledHeight = height;
        SetScale(height / GetReferenceHeight());
    }

    public void SetWidth(float width)
    {
        this.scaledHeight = null;
        SetScale(width / optionWidth);
    }

    private void UpdateText()
    {
        Transform costTextTransform = transform.Find("CostText");
        bool newActive = this.costText != null;
        bool currentActive = costTextTransform.gameObject.activeSelf;
        costTextTransform.gameObject.SetActive(this.costText != null);
        costTextTransform.GetComponent<TMPro.TextMeshProUGUI>().text = costText;

        if (newActive != currentActive)
        {
            RectTransform rectTransform = transform.GetComponent<RectTransform>();
            float scale = rectTransform.rect.width / optionWidth;
            rectTransform.sizeDelta = new Vector2(
                rectTransform.rect.width,
                GetReferenceHeight() * scale
            );

            if (this.scaledHeight != null)
            {
                SetHeight(this.scaledHeight.Value);
            }
        }

        LayoutGroup parentLayoutGroup = GetComponentInParent<LayoutGroup>();
        if (parentLayoutGroup != null)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(parentLayoutGroup.GetComponent<RectTransform>());
        }
    }

    public void updateView()
    {
        transform.Find("Card").GetComponentInChildren<CardHandler>().updateView();
    }

    protected virtual void OnRectTransformDimensionsChange()
    {
        this.updateView();
        this.needTextUpdate = true;
    }

    private float GetReferenceHeight()
    {
        return this.costText != null ? optionHeight : cardHeight;
    }
}
