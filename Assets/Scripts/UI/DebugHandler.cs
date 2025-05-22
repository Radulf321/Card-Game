using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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

    public void startCombat(int difficulty) {
        Game game = new Game("Symcon");
        for (int i = 1; i < difficulty; i++) {
            game.GetCurrentCombatTarget().IncreaseLevel();
        }
        SceneManager.LoadScene("CombatScene");
    }

    public void startGame() {
        Game game = new Game("Symcon");
        game.StartRound();
    }

    public void startDialog() {
        DialogHandler.firstDialog = new DialogText("Initial", new DialogSelect("Test", new List<DialogOption>() {
            new DialogOption("Blabla", () => {
                new DialogText("Blabla", () => { SceneManager.LoadScene("DebugScene"); }).ShowDialog(); 
            }),
            new DialogOption("Show me cards", () => {
                new DialogSelect("Select one", new List<DialogOption>() {
                    new DialogOption("First", () => {
                        new DialogText("First Card", () => {
                            SceneManager.LoadScene("DebugScene");
                        }).ShowDialog(); 
                    }, "Select the first card", "Placeholder"),
                    new DialogOption("Second", () => {
                        new DialogText("Second Card", () => {
                            SceneManager.LoadScene("DebugScene");
                        }).ShowDialog(); 
                    }, "Select the second card", "Placeholder")
                }, SelectType.Cards).ShowDialog(); 
            }),
            new DialogOption("Walk away", () => {
                SceneManager.LoadScene("DebugScene");
            }),
        }));
        SceneManager.LoadScene("DialogScene");
    }

    public void startTalent() {
        Game game = new Game("Symcon");
        game.GetCurrentCombatTarget().IncreaseExperience("comfort", 10);
        game.GetCurrentCombatTarget().GetTalent("introduction").Purchase();
        SceneManager.LoadScene("TalentTreeScene");
    }
}
