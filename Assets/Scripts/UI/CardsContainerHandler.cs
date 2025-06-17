using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    where HANDLER : MonoBehaviour, IViewUpdater
{
    public GameObject cardPrefab;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void updateView()
    {
        List<CARDDATA> cards = GetCardData();
        this.updateChildrenViews<GenericCardsContainerHandler<CARDDATA, HANDLER>, HANDLER, CARDDATA>(
            cards,
            (CARDDATA cardData) =>
            {
                GameObject cardObject = Instantiate(cardPrefab);
                SetHandlerData(GetHandler(cardObject.transform), cardData);
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
            float cardWidth = GetComponent<RectTransform>().rect.height * cardPrefab.GetComponent<AspectRatioFitter>().aspectRatio;
            float myWidth = GetComponent<RectTransform>().rect.width;

            float totalCardWidth = cardWidth * cards.Count;
            float totalSpacing = myWidth - totalCardWidth;
            float spacingPerCard = Mathf.Min(totalSpacing / (cards.Count - 1), myWidth * 0.02f);

            transform.GetComponent<HorizontalLayoutGroup>().spacing = spacingPerCard;
        }
    }

    abstract protected List<CARDDATA> GetCardData();
    abstract protected void SetHandlerData(HANDLER handler, CARDDATA cardData);
    abstract protected CARDDATA GetHandlerData(HANDLER handler);
    abstract protected HANDLER GetHandler(Transform transform);
}
