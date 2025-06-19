using System;
using System.Threading.Tasks;
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
    private Dialog? nextDialog;
    private Action? nextOnFinish;

    private TaskCompletionSource<bool>? completeOnClick;

    public GameObject? dialogOptionPrefab;

    public void Initialize()
    {
        DialogHandler.Instance = this;
        SceneManager.activeSceneChanged += (Scene scene, Scene previousScene) =>
        {
            transform.gameObject.SetActive(this.nextDialog != null);
            if (this.nextDialog != null)
            {
                _ = StartDialog(this.nextDialog, changeScene: false, onFinish: this.nextOnFinish);
                this.nextDialog = null;
                this.nextOnFinish = null;
            }
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

    public async Task StartDialog(Dialog dialog, bool changeScene = true, Action? onFinish = null)
    {
        await new DialogImage(
            backgroundImagePath: "",
            leftCharacterImageData: new CharacterImageData(imagePath: ""),
            rightCharacterImageData: new CharacterImageData(imagePath: "")
        ).ShowDialog();
        if (changeScene)
        {
            this.nextDialog = dialog;
            this.nextOnFinish = onFinish ?? EndDialog;
            FadeHandler.Instance!.LoadScene("DialogScene");
        }
        else
        {
            transform.gameObject.SetActive(true);
            Dialog.CurrentDialog = dialog;
            await dialog.ShowDialog();
            (onFinish ?? EndDialog).Invoke();
            Dialog.CurrentDialog = null;
        }
    }

    public async Task ShowText(DialogText dialog)
    {
        transform.Find("SelectArea").gameObject.SetActive(false);
        transform.Find("RewardArea").gameObject.SetActive(false);

        Transform textArea = transform.Find("TextArea");
        textArea.gameObject.SetActive(true);
        textArea.Find("Text").GetComponent<TMPro.TextMeshProUGUI>().text = dialog.GetText();
        transform.Find("LeftCharacterImage").GetComponent<UnityEngine.UI.Image>().color = dialog.GetSpeaker() == "left" ? Color.white : Color.gray;
        transform.Find("RightCharacterImage").GetComponent<UnityEngine.UI.Image>().color = dialog.GetSpeaker() == "right" ? Color.white : Color.gray;
        await waitForPointerDown();
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
    }

    public Task<DialogOption> ShowSelect(DialogSelect dialog)
    {
        transform.Find("TextArea").gameObject.SetActive(false);
        transform.Find("RewardArea").gameObject.SetActive(false);

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

        Transform infoArea = selectArea.Find("Info");
        infoArea.gameObject.SetActive(dialog.IsShowUI());
        if (dialog.IsShowUI())
        {
            IntVariable amount = new IntVariable();
            amount.Value = Game.Instance.GetRemainingRounds();
            LocalizedString localizedString = infoArea.Find("Turn").GetComponent<LocalizeStringEvent>().StringReference;
            localizedString.Add("amount", amount);
            localizedString.RefreshString();
            String currencyString = "";
            foreach (String currencyID in Game.Instance.GetCurrencies())
            {
                int currencyAmount = Game.Instance.GetPlayer().GetCurrency(currencyID);
                currencyString += "   " + Game.Instance.GetCurrencyInlineIcon(currencyID) + " " + currencyAmount;
            }
            infoArea.Find("Currency").GetComponent<TMPro.TextMeshProUGUI>().text = currencyString;
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
            if (selectArea.GetChild(i).name == "Info")
            {
                continue;
            }
            Destroy(selectArea.GetChild(i).gameObject);
        }

        selectArea.Find("CardArea").gameObject.SetActive(dialog.GetSelectType() == SelectType.Cards);
        TaskCompletionSource<DialogOption> optionSelected = new TaskCompletionSource<DialogOption>();
        switch (dialog.GetSelectType())
        {
            case SelectType.Buttons:
                // Add new options
                foreach (DialogOption option in dialog.GetOptions())
                {
                    GameObject newOption = Instantiate(this.dialogOptionPrefab!, selectArea);
                    newOption.GetComponent<DialogOptionHandler>().SetDialogOption(option);
                    newOption.GetComponent<DialogOptionHandler>().SetSelectedTask(optionSelected);
                    newOption.GetComponent<LayoutElement>().preferredHeight = selectRect.height * 0.1f;
                }
                break;

            case SelectType.Cards:
                // Remove all current cards
                DialogCardActionAreaHandler cardHandler = selectArea.Find("CardArea").GetComponent<DialogCardActionAreaHandler>();
                cardHandler.SetDialogOptions(dialog.GetOptions());
                cardHandler.SetSelectedTask(optionSelected);
                break;
        }

        return optionSelected.Task;
    }

    public async Task ShowReward(DialogReward dialog)
    {
        transform.Find("SelectArea").gameObject.SetActive(false);
        transform.Find("TextArea").gameObject.SetActive(false);

        Transform rewardArea = transform.Find("RewardArea");
        rewardArea.gameObject.SetActive(true);
        rewardArea.Find("RewardContainer").Find("RewardDisplayArea").GetComponent<RewardAreaHandler>().SetRewards(dialog.GetRewards());

        await waitForPointerDown();
    }

    public void EndDialog()
    {
        transform.gameObject.SetActive(false);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (this.completeOnClick != null)
        {
            TaskCompletionSource<bool> completeOnClick = this.completeOnClick;
            this.completeOnClick = null;
            completeOnClick.SetResult(true);
        }
    }

    private Task<bool> waitForPointerDown()
    {
        if (this.completeOnClick != null)
        {
            throw new System.Exception("Already waiting for pointer down.");
        }
        this.completeOnClick = new TaskCompletionSource<bool>();
        return completeOnClick.Task;
    }
}
