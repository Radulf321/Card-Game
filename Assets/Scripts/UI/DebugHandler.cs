using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using UnityEngine;

public class DebugHandler : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void startCombat(int difficulty)
    {
        Game game = new Game("Symcon");
        for (int i = 1; i < difficulty; i++)
        {
            game.GetCurrentCombatTarget().IncreaseLevel();
        }
        FadeHandler.Instance!.LoadScene("CombatScene");
    }

    public void startGame()
    {
        Game game = new Game("Symcon");
        game.StartGame();
    }

    public async void startDialog()
    {
        Game game = new Game("Symcon");
        game.GetPlayer().AddCurrency("cash", 100);
        JObject json = new JObject();
        json["left"] = "Player";
        DialogImage dialogImage = new DialogImage(
            json
        );
        await DialogHandler.Instance.StartDialog(new DialogText("Initial", nextDialog: new DialogSelect("Test", new List<DialogOption>() {
            new DialogOption("Blabla",
                new DialogSelect("Test 2", new List<DialogOption>() {
                    new DialogOption("Blabla 2",
                        new DialogText("Blabla")
                    )
                })
            ),
            new DialogOption("Show me cards",
                new DialogSelect("Select one", new List<DialogOption>() {
                    new DialogOption("First",
                        new DialogText("First Card"), "Select the first card", "Placeholder"),
                    new DialogOption("Second",
                        new DialogText("Second Card"), "Select the second card", "Placeholder")
                }, SelectType.Cards)
            ),
            new DialogOption("Show me cards with cost",
                new DialogSelect("Select one", options: new List<DialogOption>() {
                    new DialogOption(card: Game.Instance.GetCard("optimizeObjectTree"),
                        dialog: new DialogText("First Card"), cost: new Dictionary<string, int>()),
                    new DialogOption("Second",
                        new DialogText("Second Card"), "Have the money", "Placeholder", cost: new Dictionary<string, int>(){
                            {
                                "cash", 50
                            }
                        }),
                    new DialogOption("Third",
                        new DialogText("Third Card"), "Too expensive", "Placeholder", cost: new Dictionary<string, int>(){
                            {
                                "cash", 500
                            }
                        })
                }, selectType: SelectType.Cards, showUI: true)
            ),
            new DialogOption("Walk away")
        }
        )), onFinish: () =>
        {
            FadeHandler.Instance!.LoadScene("DebugScene");
        });
    }

    public void startTalent()
    {
        Game game = new Game("Symcon");
        CombatTarget combatTarget = game.GetCharacterManager().GetCombatTarget("singleFamilyHome");
        game.SetCurrentCombatTarget(combatTarget);
        combatTarget.GetTalent("introduction")?.Purchase(() => { });
        combatTarget.IncreaseExperience("comfort", 10);
        FadeHandler.Instance!.LoadScene("TalentTreeScene");
    }

    public void startEquipment()
    {
        Game game = new Game("Symcon");
        FadeHandler.Instance!.LoadScene("GamePreparationScene");
    }

    public void startEndGame()
    {
        Game game = new Game("Symcon");
        game.GetTaskManager().Initialize();
        for (int i = 0; i < 3; i++)
        {
            game.SendTriggerMessage(new TriggerMessage(TriggerType.EndCombat, new TriggerMessageData(success: true)));
        }
        FadeHandler.Instance!.LoadScene("GameEndScene");
    }

    public void resetPlayerPreferences()
    {
        PlayerPrefs.DeleteAll();
        _ = DialogHandler.Instance.StartDialog(new DialogText("All data cleared"), changeScene: false);
    }

    public void resetPermanentFlags()
    {
        PlayerPrefs.DeleteKey("Symcon" + Game.permanentFlagsKey);
        _ = DialogHandler.Instance.StartDialog(new DialogText("Permanent Flags cleared"), changeScene: false);
    }

    public void StartCardGallery()
    {
        Game game = new Game("Symcon");
        FadeHandler.Instance!.LoadScene("CardGalleryScene");
    }

    public void CompleteAllTasks()
    {
        Game game = new Game("Symcon");
        game.GetTaskManager().Initialize();
        game.GetTaskManager().DebugCompleteAllTasks();
        _ = DialogHandler.Instance.StartDialog(new DialogText("All tasks completed"), changeScene: false);

    }
}
