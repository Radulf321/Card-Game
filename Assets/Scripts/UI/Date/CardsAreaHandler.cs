using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardsAreaHandler : MonoBehaviour, IViewUpdater
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
        List<Card> cards = CombatHandler.instance.getCardPile().GetHand();
        this.updateChildrenViews<CardsAreaHandler, CardHandler, Card>(
            cards,
            (Card card) =>
            {
                GameObject cardObject = Instantiate(cardPrefab);
                cardObject.GetComponentInChildren<CardHandler>().SetCard(card);
                return cardObject;
            },
            (CardHandler cardHandler) => cardHandler.GetCard(),
            getChildComponent: (index) => {
                Debug.Log(index);
                Debug.Log(transform.GetChild(index));
                Debug.Log(transform.GetChild(index).GetComponentInChildren<CardHandler>());
                return transform.GetChild(index).GetComponentInChildren<CardHandler>();
            }
        );

        if (cards.Count > 1)
        {
            float cardWidth = GetComponent<RectTransform>().rect.height * 0.75f;
            float myWidth = GetComponent<RectTransform>().rect.width;

            float totalCardWidth = cardWidth * cards.Count;
            float totalSpacing = myWidth - totalCardWidth;
            float spacingPerCard = totalSpacing / (cards.Count - 1);
            if (spacingPerCard > 20)
            {
                spacingPerCard = 20;
            }

            transform.GetComponent<HorizontalLayoutGroup>().spacing = spacingPerCard;
        }

    }
}
