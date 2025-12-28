using UnityEngine;
using UnityEngine.Localization.Settings;

public class MainMenuHandler : MonoBehaviour
{
    public string resourcePath = "Symcon";

    private bool update = true;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (update)
        {
            bool tutorialDone = PlayerPrefs.GetInt(this.resourcePath + Game.tutorialDoneKey, 0) == 1;
            string startText = tutorialDone ? "StartRun" : "StartGame";
            Transform buttonContainer = transform.Find("ButtonContainer");
            LocalizationSettings.StringDatabase.GetLocalizedStringAsync("UIStrings", startText).Completed += (handle) =>
            {
                buttonContainer.Find("StartButton").GetComponentInChildren<TMPro.TextMeshProUGUI>().text = handle.Result;
            };
            bool hasCurrentRun = PlayerPrefs.HasKey(this.resourcePath + Game.saveGameKey);
            buttonContainer.Find("ContinueButton").GetComponent<UnityEngine.UI.Button>().interactable = hasCurrentRun;
            update = false;
        }
    }

    public void StartGame()
    {
        Game game = new Game(this.resourcePath);
        game.StartGame();
    }

    public void ContinueGame()
    {
        Game game = new Game(this.resourcePath);
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
        Game game = new Game(this.resourcePath);
        FadeHandler.Instance!.LoadScene("CardGalleryScene");
    }
}
