using System;
using System.Collections.Generic;
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

    public static void StartDialog(Dialog dialog)
    {
        DialogHandler.Instance!.firstDialog = dialog;
        FadeHandler.Instance!.LoadScene("DialogScene");
    }

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

    public void ShowText(DialogText dialog) {
        transform.Find("SelectArea").gameObject.SetActive(false);
        transform.Find("RewardArea").gameObject.SetActive(false);

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
        if (dialog.IsShowUI()) {
            IntVariable amount = new IntVariable();
            amount.Value = Game.Instance.GetRemainingRounds();
            LocalizedString localizedString = turnInfo.GetComponent<LocalizeStringEvent>().StringReference;
            localizedString.Add("amount", amount);
            localizedString.RefreshString();
        }
        // Remove all current options
        for (int i = 0; i < selectArea.childCount; i++) {
            if (selectArea.GetChild(i).name == "Title") {
                continue;
            }
            if (selectArea.GetChild(i).name == "CardArea") {
                continue;
            }
            if (selectArea.GetChild(i).name == "TurnInfo") {
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
