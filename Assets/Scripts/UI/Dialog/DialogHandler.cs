using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

#nullable enable
public class DialogHandler : MonoBehaviour, IPointerDownHandler
{
    public static Dialog? firstDialog;
    public static Action? dialogFinish;

    public static void StartDialog(Dialog dialog) {
        firstDialog = dialog;
        SceneManager.LoadScene("DialogScene");
    }

    private Action? onClickAction;

    public GameObject? dialogOptionPrefab;
    public GameObject? dialogCardPrefab;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        firstDialog!.ShowDialog();    
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ShowText(DialogText dialog) {
        transform.Find("SelectArea").gameObject.SetActive(false);

        Transform textArea = transform.Find("TextArea");
        textArea.gameObject.SetActive(true);
        textArea.Find("Text").GetComponent<TMPro.TextMeshProUGUI>().text = dialog.GetText();
        transform.Find("LeftCharacterImage").GetComponent<UnityEngine.UI.Image>().color = dialog.GetSpeaker() == "left" ? Color.white : Color.gray;
        transform.Find("RightCharacterImage").GetComponent<UnityEngine.UI.Image>().color = dialog.GetSpeaker() == "right" ? Color.white : Color.gray;
        this.onClickAction = dialog.GetOnFinish();
    }

    public void ShowImage(DialogImage dialog) {
        if (dialog.GetBackgroundImagePath() != null) {
            transform.Find("BackgroundImage").GetComponent<UnityEngine.UI.Image>().sprite = Resources.Load<Sprite>(dialog.GetBackgroundImagePath());
        }
        if (dialog.GetLeftCharacterImagePath() != null) {
            Transform leftCharacter = transform.Find("LeftCharacterImage");
            leftCharacter.gameObject.SetActive(dialog.GetLeftCharacterImagePath() != "");
            if (dialog.GetLeftCharacterImagePath() != "") {
                leftCharacter.GetComponent<UnityEngine.UI.Image>().sprite = Resources.Load<Sprite>(dialog.GetLeftCharacterImagePath());
            }
        }
        if (dialog.GetRightCharacterImagePath() != null) {
            Transform rightCharacter = transform.Find("RightCharacterImage");
            rightCharacter.gameObject.SetActive(dialog.GetRightCharacterImagePath() != "");
            if (dialog.GetRightCharacterImagePath() != "") {
                rightCharacter.GetComponent<UnityEngine.UI.Image>().sprite = Resources.Load<Sprite>(dialog.GetRightCharacterImagePath());
            }
        }
        dialog.GetOnFinish()();
    }

    public void ShowSelect(DialogSelect dialog) {
        transform.Find("TextArea").gameObject.SetActive(false);
        this.onClickAction = null;

        Transform selectArea = transform.Find("SelectArea");
        Rect selectRect = selectArea.GetComponent<RectTransform>().rect;
        VerticalLayoutGroup selectLayoutGroup = selectArea.GetComponent<VerticalLayoutGroup>();
        selectLayoutGroup.spacing = selectRect.height * 0.01f;
        selectLayoutGroup.padding = new RectOffset(
            Mathf.FloorToInt(selectRect.width * 0.02f),
            Mathf.FloorToInt(selectRect.width * 0.02f),
            Mathf.FloorToInt(selectRect.height * 0.02f),
            Mathf.FloorToInt(selectRect.height * 0.02f)
        );
        selectArea.gameObject.SetActive(true);
        Transform titleTransform = selectArea.Find("Title");
        titleTransform.GetComponent<TMPro.TextMeshProUGUI>().text = dialog.GetTitle();
        titleTransform.GetComponent<LayoutElement>().preferredHeight = selectRect.height * 0.1f;
        // Remove all current options
        for (int i = 0; i < selectArea.childCount; i++) {
            if (selectArea.GetChild(i).name == "Title") {
                continue;
            }
            if (selectArea.GetChild(i).name == "CardArea") {
                continue;
            }
            Destroy(selectArea.GetChild(i).gameObject);
        }

        selectArea.Find("CardArea").gameObject.SetActive(dialog.GetSelectType() == SelectType.Cards);
        switch (dialog.GetSelectType()) {
            case SelectType.Buttons:
                // Add new options
                foreach (DialogOption option in dialog.GetOptions()) {
                    GameObject newOption = Instantiate(this.dialogOptionPrefab!, selectArea);
                    newOption.GetComponent<DialogOptionHandler>().SetText(option.GetTitle());
                    newOption.GetComponent<DialogOptionHandler>().SetAction(option.GetAction());
                    newOption.GetComponent<LayoutElement>().preferredHeight = selectRect.height * 0.1f;
                }
                break;

            case SelectType.Cards:
                // Remove all current cards
                Transform cardArea = selectArea.Find("CardArea");
                for (int i = 0; i < cardArea.childCount; i++) {
                    Destroy(cardArea.GetChild(i).gameObject);
                }
                foreach (DialogOption option in dialog.GetOptions()) {
                    GameObject newCard = Instantiate(this.dialogCardPrefab!, cardArea);
                    newCard.GetComponentInChildren<CardHandler>().SetTitle(option.GetTitle());
                    newCard.GetComponentInChildren<CardHandler>().SetDescription(option.GetDescription());
                    newCard.GetComponentInChildren<CardHandler>().SetSprite(Resources.Load<Sprite>(option.GetImagePath()));
                    newCard.GetComponentInChildren<CardHandler>().SetCostSprite(null);
                    newCard.GetComponentInChildren<CardHandler>().SetOnClickAction(option.GetAction());
                }
                break;
        }

    }

    public void OnPointerDown(PointerEventData eventData)
    {
        this.onClickAction?.Invoke();
    }
}
