using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class CardsContainerHandler<CARDDATA> : MonoBehaviour, IViewUpdater
    where CARDDATA: class
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
        this.updateChildrenViews<CardsContainerHandler<CARDDATA>, CardHandler, CARDDATA>(
            cards,
            (CARDDATA cardData) =>
            {
                GameObject cardObject = Instantiate(cardPrefab);
                SetHandlerData(cardObject.GetComponentInChildren<CardHandler>(), cardData);
                return cardObject;
            },
            GetHandlerData,
            getChildComponent: (index) => {
                return transform.GetChild(index).GetComponentInChildren<CardHandler>();
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
    abstract protected void SetHandlerData(CardHandler cardHandler, CARDDATA cardData);
    abstract protected CARDDATA GetHandlerData(CardHandler cardHandler);
}
