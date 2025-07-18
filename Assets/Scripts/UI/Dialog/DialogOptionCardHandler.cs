using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

#nullable enable

public class DialogOptionCardHandler : MonoBehaviour, IViewUpdater
{
    private static float aspectRatioWithCost = 0.6f;
    private static float cardOffsetWithCost = 0.2f;

    private bool needTextUpdate = false;
    private DialogOption? option;
    private Action? onClick;
    private string? costText;
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

    private void UpdateText()
    {
        AspectRatioFitter aspectRatioFitter = transform.GetComponent<AspectRatioFitter>();
        Transform costTextTransform = transform.Find("CostText");
        Transform cardTransform = transform.Find("Card");
        LayoutElement layoutElement = transform.GetComponent<LayoutElement>();
        float height = transform.GetComponent<RectTransform>().rect.height;
        aspectRatioFitter.aspectRatio = DialogOptionCardHandler.aspectRatioWithCost;
        cardTransform.GetComponent<RectTransform>().anchorMin = new Vector2(0, DialogOptionCardHandler.cardOffsetWithCost);
        costTextTransform.gameObject.SetActive(true);
        costTextTransform.GetComponent<TMPro.TextMeshProUGUI>().text = costText;
        layoutElement.preferredWidth = height * aspectRatioFitter.aspectRatio;

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
}
