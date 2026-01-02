using UnityEngine;
using System.Linq;

#nullable enable

public class CardGalleryHandler : MonoBehaviour
{
    public GameObject cardPrefab;
    public GameObject actionPrefab;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        RenderCards();
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void BackToMain()
    {
        FadeHandler.Instance?.LoadScene("MainMenuScene");
    }

    private void RenderCards()
    {
        Rect parentSize = GameObject.Find("Canvas").transform.GetComponent<RectTransform>().rect;
        float cardHeight = Mathf.Max(240f, parentSize.height * 0.25f);
        int cardsPerColumn = Mathf.FloorToInt(parentSize.height / (cardHeight + 20));
        if (cardsPerColumn < 1)
        {
            cardsPerColumn = 1;
        }

        float currentX = 20;
        float currentY = 20;
        int columnIndex = 0;

        for (int i = 0; i < transform.childCount; i++)
        {
            Destroy(transform.GetChild(i).gameObject);
        }

        foreach (Card card in Game.Instance.GetAllCards())
        {
            GameObject cardObject = Instantiate(cardPrefab);
            cardObject.transform.SetParent(this.transform, false);
            CardHandler cardHandler = cardObject.GetComponentInChildren<CardHandler>();
            cardHandler.SetCard(card);
            cardHandler.SetHeight(cardHeight);
            RectTransform cardRect = cardObject.GetComponent<RectTransform>();
            cardRect.anchorMin = new Vector2(0, 1);
            cardRect.anchorMax = new Vector2(0, 1);
            cardRect.pivot = new Vector2(0, 1);
            cardRect.anchoredPosition = new Vector2(currentX, -currentY);
            currentY += cardHeight + 20;
            columnIndex++;
            if (columnIndex >= cardsPerColumn)
            {
                currentY = 20;
                currentX += cardHeight;
                columnIndex = 0;
            }
        }

        currentX += cardHeight;
        float actionHeight = Mathf.Max(512f, parentSize.height * 0.66f);

        foreach (ActionCharacter target in Game.Instance.GetCharacterManager().GetAllCombatTargets().Cast<ActionCharacter>()
            .Concat(Game.Instance.GetCharacterManager().GetAllLocations().Cast<ActionCharacter>()))
        {
            DialogOption? action = target.GetDialogOption();
            if (action == null)
            {
                continue;
            }

            GameObject actionObject = Instantiate(actionPrefab);
            actionObject.transform.SetParent(this.transform, false);
            DialogOptionCardHandler dialogHandler = actionObject.GetComponent<DialogOptionCardHandler>();
            dialogHandler.SetOption(action);
            dialogHandler.SetHeight(actionHeight);
            RectTransform actionRect = actionObject.GetComponent<RectTransform>();
            actionRect.anchorMin = new Vector2(0, 1);
            actionRect.anchorMax = new Vector2(0, 1);
            actionRect.pivot = new Vector2(0, 1);
            actionRect.anchoredPosition = new Vector2(currentX, (-parentSize.height / 2) + (actionHeight / 2));
            currentX += actionHeight;
        }

        GetComponent<RectTransform>().sizeDelta = new Vector2(Mathf.Max(parentSize.width, currentX + cardHeight), parentSize.height);
    }
}
