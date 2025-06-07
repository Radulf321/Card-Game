using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;
using UnityEngine.Localization.SmartFormat.PersistentVariables;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

#nullable enable
public class DialogHandler : MonoBehaviour, IPointerDownHandler
{
    public static DialogHandler? Instance;
    public static Action? dialogFinish;
    public Dialog? firstDialog;

    private Action? onClickAction;

    public GameObject? dialogOptionPrefab;

    public void Initialize()
    {
        UnityEngine.Debug.Log("DialogHandler Awake called");
        DialogHandler.Instance = this;
        SceneManager.activeSceneChanged += (Scene scene, Scene previousScene) =>
        {
            transform.gameObject.SetActive(this.firstDialog != null);
            this.firstDialog?.ShowDialog();
            this.firstDialog = null;
        };
        // Ensure the dialogOptionPrefab is assigned
        if (dialogOptionPrefab == null)
        {
            Debug.LogError("DialogOptionPrefab is not assigned in DialogHandler.");
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void StartDialog(Dialog dialog, bool changeScene = true)
    {
        new DialogImage(
            backgroundImagePath: "",
            leftCharacterImagePath: "",
            rightCharacterImagePath: "",
            onFinish: () => { }
        ).ShowDialog();
        if (changeScene)
        {
            this.firstDialog = dialog;
            FadeHandler.Instance!.LoadScene("DialogScene");
        }
        else
        {
            transform.gameObject.SetActive(true);
            dialog.ShowDialog();
        }
    }

    public void ShowText(DialogText dialog)
    {
        transform.Find("SelectArea").gameObject.SetActive(false);
        transform.Find("RewardArea").gameObject.SetActive(false);

        Transform textArea = transform.Find("TextArea");
        textArea.gameObject.SetActive(true);
        textArea.Find("Text").GetComponent<TMPro.TextMeshProUGUI>().text = dialog.GetText();
        transform.Find("LeftCharacterImage").GetComponent<UnityEngine.UI.Image>().color = dialog.GetSpeaker() == "left" ? Color.white : Color.gray;
        transform.Find("RightCharacterImage").GetComponent<UnityEngine.UI.Image>().color = dialog.GetSpeaker() == "right" ? Color.white : Color.gray;
        this.onClickAction = dialog.GetOnFinish();
    }

    public void ShowImage(DialogImage dialog)
    {
        if (dialog.GetBackgroundImagePath() != null)
        {
            Image backgroundImage = transform.Find("BackgroundImage").GetComponent<Image>();
            if (dialog.GetBackgroundImagePath() != "")
            {
                backgroundImage.sprite = Resources.Load<Sprite>(dialog.GetBackgroundImagePath());
                backgroundImage.color = Color.white;
            }
            else
            {
                backgroundImage.sprite = null;
                backgroundImage.color = Color.clear;
            }
        }

        Action<CharacterImageData, string> handleCharacterImage = (CharacterImageData imageData, string imageID) =>
        {
            string? imagePath = imageData.GetImagePath();
            if (imagePath == null)
            {
                return;
            }
            Transform characterImageTransform = transform.Find(imageID);
            characterImageTransform.gameObject.SetActive(imagePath != "");
            if (imagePath != "")
            {
                characterImageTransform.GetComponent<UnityEngine.UI.Image>().sprite = Resources.Load<Sprite>(imagePath);
                RectTransform rectTransform = characterImageTransform.GetComponent<RectTransform>();
                switch (imageData.GetAlignment())
                {
                    case VerticalAlignment.Top:
                        rectTransform.anchorMin = new Vector2(rectTransform.anchorMin.x, 0.2f);
                        rectTransform.anchorMax = new Vector2(rectTransform.anchorMax.x, 1f);
                        break;

                    case VerticalAlignment.Bottom:
                        rectTransform.anchorMin = new Vector2(rectTransform.anchorMin.x, 0f);
                        rectTransform.anchorMax = new Vector2(rectTransform.anchorMax.x, 0.8f);
                        break;

                    case VerticalAlignment.Center:
                        rectTransform.anchorMin = new Vector2(rectTransform.anchorMin.x, 0.1f);
                        rectTransform.anchorMax = new Vector2(rectTransform.anchorMax.x, 0.9f);
                        break;
                }
                if (imageData.GetMirror())
                {
                    if (rectTransform.localScale.x != -1f)
                    {
                        rectTransform.localScale = new Vector3(-1f, 1f, 1f);
                        rectTransform.pivot = new Vector2(1 - rectTransform.pivot.x, rectTransform.pivot.y);
                    }
                }
                else
                {
                    if (rectTransform.localScale.x != 1f)
                    {
                        rectTransform.localScale = new Vector3(1f, 1f, 1f);
                        rectTransform.pivot = new Vector2(1 - rectTransform.pivot.x, rectTransform.pivot.y);
                    }
                }
            }
        };

        handleCharacterImage(dialog.GetLeftCharacterImageData(), "LeftCharacterImage");
        handleCharacterImage(dialog.GetRightCharacterImageData(), "RightCharacterImage");

        dialog.GetOnFinish()();
    }

    public void ShowSelect(DialogSelect dialog)
    {
        transform.Find("TextArea").gameObject.SetActive(false);
        transform.Find("RewardArea").gameObject.SetActive(false);
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

        Transform turnInfo = selectArea.Find("TurnInfo");
        turnInfo.gameObject.SetActive(dialog.IsShowUI());
        if (dialog.IsShowUI())
        {
            IntVariable amount = new IntVariable();
            amount.Value = Game.Instance.GetRemainingRounds();
            LocalizedString localizedString = turnInfo.GetComponent<LocalizeStringEvent>().StringReference;
            localizedString.Add("amount", amount);
            localizedString.RefreshString();
        }
        // Remove all current options
        for (int i = 0; i < selectArea.childCount; i++)
        {
            if (selectArea.GetChild(i).name == "Title")
            {
                continue;
            }
            if (selectArea.GetChild(i).name == "CardArea")
            {
                continue;
            }
            if (selectArea.GetChild(i).name == "TurnInfo")
            {
                continue;
            }
            Destroy(selectArea.GetChild(i).gameObject);
        }

        selectArea.Find("CardArea").gameObject.SetActive(dialog.GetSelectType() == SelectType.Cards);
        switch (dialog.GetSelectType())
        {
            case SelectType.Buttons:
                // Add new options
                foreach (DialogOption option in dialog.GetOptions())
                {
                    GameObject newOption = Instantiate(this.dialogOptionPrefab!, selectArea);
                    newOption.GetComponent<DialogOptionHandler>().SetText(option.GetTitle());
                    newOption.GetComponent<DialogOptionHandler>().SetAction(option.GetAction());
                    newOption.GetComponent<LayoutElement>().preferredHeight = selectRect.height * 0.1f;
                }
                break;

            case SelectType.Cards:
                // Remove all current cards
                selectArea.Find("CardArea").GetComponent<DialogCardActionAreaHandler>().SetDialogOptions(dialog.GetOptions());
                break;
        }

    }

    public void ShowReward(DialogReward dialog)
    {
        transform.Find("SelectArea").gameObject.SetActive(false);
        transform.Find("TextArea").gameObject.SetActive(false);

        Transform rewardArea = transform.Find("RewardArea");
        rewardArea.gameObject.SetActive(true);
        rewardArea.Find("RewardContainer").Find("RewardDisplayArea").GetComponent<RewardAreaHandler>().SetRewards(dialog.GetRewards());
        this.onClickAction = dialog.GetOnFinish();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        this.onClickAction?.Invoke();
    }
}
