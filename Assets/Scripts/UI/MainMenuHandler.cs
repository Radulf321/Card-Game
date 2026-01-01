using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;
using TMPro;

public class MainMenuHandler : MonoBehaviour
{
    public GameObject resourceButtonPrefab;
    public GameObject resourceSelectionCanvas;

    private string resourcePath;
    private bool update = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        string resourcesPath = System.IO.Path.Combine(Application.dataPath, "Resources");
        string[] subfolders = System.IO.Directory.GetDirectories(resourcesPath);
        
        if (subfolders.Length > 1)
        {
            ShowSubfolderSelection(subfolders);
        }
        else if (subfolders.Length == 1)
        {
            SelectSubfolder(System.IO.Path.GetFileName(subfolders[0]));
        }
    }

    private void ShowSubfolderSelection(string[] subfolders)
    {
        resourceSelectionCanvas.SetActive(true);
        Transform buttonContainer = resourceSelectionCanvas.transform.Find("ButtonContainer");
        for (int i = 0; i < subfolders.Length; i++)
        {
            string folderName = System.IO.Path.GetFileName(subfolders[i]);
            GameObject buttonInstance = Instantiate(resourceButtonPrefab, buttonContainer);
            Button button = buttonInstance.GetComponent<Button>();
            button.onClick.AddListener(() => { 
                resourceSelectionCanvas.SetActive(false);
                SelectSubfolder(folderName); 
            });
            // Set button text if it has a child TextMeshProUGUI
            TextMeshProUGUI buttonText = buttonInstance.GetComponentInChildren<TextMeshProUGUI>();
            buttonText.text = folderName;
        }
    }

    private void SelectSubfolder(string folderName)
    {
        resourcePath = folderName;
        update = true;
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
