using System.Collections.Generic;
using UnityEngine;

public class TurnsAreaHandler : MonoBehaviour, IViewUpdater
{
    public GameObject turnPrefab;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void updateView() {
        this.updateChildrenViews<TurnsAreaHandler, TurnHandler, Turn>(
            CombatHandler.instance.getTurns(),
            (Turn turn) => {
                GameObject turnObject = Instantiate(turnPrefab);
                turnObject.GetComponent<TurnHandler>().SetTurn(turn);
                return turnObject;
            },
            (turnHandler) => turnHandler.GetTurn()
        );
    }
}
