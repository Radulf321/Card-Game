using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

#nullable enable

public abstract class CardsContainerHandler<CARDDATA> : GenericCardsContainerHandler<CARDDATA, CardHandler>
    where CARDDATA : class
{
    protected override CardHandler GetHandler(Transform transform)
    {
        return transform.GetComponentInChildren<CardHandler>();
    }
}

public abstract class GenericCardsContainerHandler<CARDDATA, HANDLER> : MonoBehaviour, IViewUpdater
    where CARDDATA : class
    where HANDLER : MonoBehaviour, IViewUpdater, IScalable
{
    public GameObject cardPrefab;
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
            updateView();
            this.shouldUpdate = false;
        }
    }

    public void updateView()
    {
        List<CARDDATA> cards = GetCardData();
        this.updateChildrenViews<GenericCardsContainerHandler<CARDDATA, HANDLER>, HANDLER, CARDDATA>(
            cards,
            (CARDDATA cardData) =>
            {
                GameObject cardObject = Instantiate(cardPrefab);
                HANDLER cardHandler = GetHandler(cardObject.transform);
                cardHandler.SetHeight(transform.GetComponent<RectTransform>().rect.height);
                SetHandlerData(cardHandler, cardData);
                return cardObject;
            },
            GetHandlerData,
            getChildComponent: (index) =>
            {
                return GetHandler(transform.GetChild(index));
            }
        );

        if (cards.Count > 1)
        {
            RectTransform cardRect = cardPrefab.GetComponent<RectTransform>();
            float aspectRatio = cardRect.rect.width / cardRect.rect.height;
            float cardWidth = GetComponent<RectTransform>().rect.height * aspectRatio;
            float myWidth = GetComponent<RectTransform>().rect.width;

            float totalCardWidth = cardWidth * cards.Count;
            float totalSpacing = myWidth - totalCardWidth;
            float spacingPerCard = Mathf.Min(totalSpacing / (cards.Count - 1), myWidth * 0.02f);

            transform.GetComponent<HorizontalLayoutGroup>().spacing = spacingPerCard;
        }
    }

    protected virtual void OnRectTransformDimensionsChange()
    {
        float height = transform.GetComponent<RectTransform>().rect.height;
        for (int i = 0; i < transform.childCount; i++)
        {
            HANDLER cardData = GetHandler(transform.GetChild(i));
            if (cardData == null) continue;
            cardData.SetHeight(height);
        }
        this.shouldUpdate = true;

    }

    abstract protected List<CARDDATA> GetCardData();
    abstract protected void SetHandlerData(HANDLER handler, CARDDATA cardData);
    abstract protected CARDDATA? GetHandlerData(HANDLER handler);
    abstract protected HANDLER GetHandler(Transform transform);
}
