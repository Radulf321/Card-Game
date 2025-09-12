using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Linq;
using System;
using TMPro;

#nullable enable
public class CardHandler : MonoBehaviour, IViewUpdater, IPointerDownHandler, IScalable
{
    public static float standardHeight = 1038f;
    public static float standardWidth = 778f;

    private Card? card;
    private Talent? talent;
    private Sprite? sprite;
    private Sprite? costSprite;
    private bool active = true;

    private string? title;
    private string? description;
    private string? cost;
    private Action? onClickAction;

    private bool shouldUpdate = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (this.shouldUpdate)
        {
            Transform cardContainer = transform.Find("CardContainer");
            Transform nameArea = cardContainer.Find("NameArea");
            nameArea.Find("Name").GetComponent<TMPro.TextMeshProUGUI>().text =
                this.title ?? card?.GetName() ??
                talent?.GetTitle() ?? "This should never be visible";
            updateDescription();
            Transform costArea = cardContainer.Find("CostArea");
            costArea.gameObject.SetActive(this.costSprite != null);
            if (this.costSprite != null)
            {
                Image costImage = costArea.Find("CostImage").GetComponent<Image>();
                costImage.sprite = this.costSprite;
                TMPro.TextMeshProUGUI costText = costArea.Find("Cost").GetComponent<TMPro.TextMeshProUGUI>();
                if (this.cost != null)
                {
                    costText.text = this.cost;
                }
                else if (this.card != null)
                {
                    costText.text = card!.GetCost()?.ToString() ?? "";
                }
                else if (this.talent != null)
                {
                    costText.text = talent!.IsPurchased() ? "" : talent!.GetCost().First().Value.ToString();
                }
                else {
                    throw new Exception("Could not deduce cost of card as no specific cost is set, nor any talent or card");
                }
            }
            Image image = cardContainer.Find("ImageContainer").Find("Image").GetComponent<Image>();
            if (image.sprite != this.sprite)
            {
                image.sprite = this.sprite;
            }
            this.shouldUpdate = false;
        }
    }

    protected virtual void OnRectTransformDimensionsChange()
    {
        this.shouldUpdate = true;
    }

    public void SetScale(float scale)
    {
        RectTransform containerRect = transform.Find("CardContainer").GetComponent<RectTransform>();
        RectTransform rectTransform = transform.GetComponent<RectTransform>();
        containerRect.localScale = new Vector3(scale, scale, scale);
        rectTransform.sizeDelta = new Vector2(containerRect.rect.width * scale, containerRect.rect.height * scale);
        transform.GetComponent<Image>().pixelsPerUnitMultiplier = 0.3f / scale;
        this.shouldUpdate = true;
    }

    public void SetHeight(float height)
    {
        RectTransform containerRect = transform.Find("CardContainer").GetComponent<RectTransform>();
        SetScale(height / containerRect.rect.height);
    }

    public void SetWidth(float width)
    {
        RectTransform containerRect = transform.Find("CardContainer").GetComponent<RectTransform>();
        SetScale(width / containerRect.rect.width);
    }

    public void SetCard(Card card)
    {
        this.card = card;
        this.talent = null;
        this.sprite = Resources.Load<Sprite>(card.GetImagePath());
        this.costSprite = (card.GetCardType() == CardType.Relic) ? Game.Instance.GetIcon("Relic") : Game.Instance.GetIcon("EnergyCard");
        // TODO: Think about going for NoWrap for cards, it could look nicer for multiple effects in one line each...
        GetDescriptionText().textWrappingMode = TMPro.TextWrappingModes.NoWrap;
        updateView();
    }

    public void SetTalent(Talent talent)
    {
        this.talent = talent;
        this.card = null;
        this.sprite = Resources.Load<Sprite>(talent.GetImagePath());
        if (talent.IsPurchased())
        {
            this.costSprite = Game.Instance.GetCheckIcon();
        }
        else if (talent.GetCost().Count == 0)
        {
            this.costSprite = null;
        }
        else
        {
            this.costSprite = Game.Instance.GetExperienceTypeIcon(talent.GetCost().Keys.First());
        }
        GetDescriptionText().textWrappingMode = TMPro.TextWrappingModes.Normal;
        updateView();
    }

    public void SetSprite(Sprite? sprite)
    {
        this.sprite = sprite;
        updateView();
    }

    public void SetCostSprite(Sprite? costSprite)
    {
        this.costSprite = costSprite;
        updateView();
    }

    public void SetTitle(string? title)
    {
        this.title = title;
        updateView();
    }

    public void SetDescription(string? description)
    {
        this.description = description;
        GetDescriptionText().textWrappingMode = TMPro.TextWrappingModes.Normal;
        updateView();
    }

    public void SetDescriptionFontSize(float fontSize)
    {
        GetDescriptionText().fontSize = fontSize;
        updateView();
    }

    public void SetCost(string? cost)
    {
        this.cost = cost;
        updateView();
    }

    public void SetActive(bool active)
    {
        this.active = active;
        transform.GetComponent<HoverHandler>().SetActive(active);
    }

    public void SetOnClickAction(Action? onClickAction)
    {
        this.onClickAction = onClickAction;
    }

    public void updateView()
    {
        this.shouldUpdate = true;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!active)
        {
            return;
        }

        if (onClickAction != null)
        {
            onClickAction.Invoke();
            return;
        }

        if (card != null)
        {
            if (card.CanPlay())
            {
                card.Play();
            }
            else
            {
                Debug.Log("Not enough energy to play this card.");
            }
        }
        if (talent != null)
        {
            FindAnyObjectByType<TalentInfoHandler>(FindObjectsInactive.Include).ShowTalent(talent);
            transform.parent.GetComponent<HoverHandler>().StopHover();
        }
    }

    public Card GetCard()
    {
        if (this.card == null)
        {
            throw new System.Exception("Card is null, cannot get card.");
        }
        return this.card;
    }

    public Talent GetTalent()
    {
        if (this.talent == null)
        {
            throw new System.Exception("Talent is null, cannot get talent.");
        }
        return this.talent;
    }

    public string GetTitle()
    {
        return this.title ?? card?.GetName() ?? talent?.GetTitle() ?? "This should never be visible";
    }

    private async void updateDescription()
    {
        TextMeshProUGUI descriptionText = GetDescriptionText();
        if (this.description != null)
        {
            descriptionText.text = this.description;
        }
        else if (card != null)
        {
            descriptionText.text = await card!.GetDescription();
        }
        else if (talent != null)
        {
            descriptionText.text = await talent!.GetDescription();
        }
        else
        {
            descriptionText.text = "This should never be visible";
        }
        descriptionText.ForceMeshUpdate();
        while (descriptionText.isTextOverflowing ||
            (descriptionText.textBounds.size.x >
                descriptionText.transform.GetComponent<RectTransform>().rect.width))
        {
            descriptionText.fontSize *= 0.6f;
            descriptionText.ForceMeshUpdate();
        }
    }

    private TMPro.TextMeshProUGUI GetDescriptionText()
    {
        return transform.Find("CardContainer").Find("DescriptionArea").Find("Description").GetComponent<TMPro.TextMeshProUGUI>();
    }
}
