using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;
using TMPro;
using Newtonsoft.Json.Linq;

public class MainMenuHandler : MonoBehaviour
{
    private static string RESOURCE_PATH = "CardData";
    private bool update = true;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        JObject index = JObject.Parse(Resources.Load<TextAsset>(RESOURCE_PATH + "/Index").text);
        TextMeshProFontManager.Instance!.SetDefaultFont(Resources.Load<TMPro.TMP_FontAsset>(RESOURCE_PATH + "/" + index["font"]!.ToString()));
    }

    // Update is called once per frame
    void Update()
    {
        if (update)
        {
            bool tutorialDone = PlayerPrefs.GetInt(RESOURCE_PATH + Game.tutorialDoneKey, 0) == 1;
            string startText = tutorialDone ? "StartRun" : "StartGame";
            Transform buttonContainer = transform.Find("ButtonContainer");
            LocalizationSettings.StringDatabase.GetLocalizedStringAsync("UIStrings", startText).Completed += (handle) =>
            {
                buttonContainer.Find("StartButton").GetComponentInChildren<TMPro.TextMeshProUGUI>().text = handle.Result;
            };
            bool hasCurrentRun = PlayerPrefs.HasKey(RESOURCE_PATH + Game.saveGameKey);
            buttonContainer.Find("ContinueButton").GetComponent<UnityEngine.UI.Button>().interactable = hasCurrentRun;
            update = false;
        }
    }

    public void StartGame()
    {
        Game game = new Game(RESOURCE_PATH);
        game.StartGame();
    }

    public void ContinueGame()
    {
        Game game = new Game(RESOURCE_PATH);
        try {
            game.ContinueGame();
        }
        catch (System.Exception e)
        {
            _ = DialogHandler.Instance.StartDialog(new DialogText("Cannot continue game: " + e.Message), changeScene: false);
        }
    }

    public void ResetPlayerPreferences()
    {
        PlayerPrefs.DeleteAll();
        _ = DialogHandler.Instance.StartDialog(new DialogText("All data cleared"), changeScene: false);
        update = true;
    }

    public void StartCardGallery()
    {
        Game game = new Game(RESOURCE_PATH);
        FadeHandler.Instance!.LoadScene("CardGalleryScene");
    }
}
