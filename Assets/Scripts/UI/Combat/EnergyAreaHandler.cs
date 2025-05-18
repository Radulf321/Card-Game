using System;
using UnityEngine;
using UnityEngine.UI;

public class EnergyAreaHandler : MonoBehaviour, IViewUpdater
{
    public GameObject energyPrefab;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void updateView() {
        // It is possible that the current energy is greater than the max energy,
        // for example when the player has a card that gives them more energy than the max energy
        int maxEnergy = Math.Max(CombatHandler.instance.getMaxEnergy(), CombatHandler.instance.getCurrentEnergy());
        int destroyedChildren = 0;

        // If max energy decreases, remove excess energy objects
        while ((transform.childCount - destroyedChildren) > maxEnergy) {
            Destroy(transform.GetChild(transform.childCount - 1 - destroyedChildren).gameObject); // Destroy the last child GameObject
            destroyedChildren++;
        }

        while (transform.childCount < maxEnergy) {
            // Instantiate the prefab
            GameObject energyInstance = Instantiate(energyPrefab);

            // Set the parent to this GameObject
            energyInstance.transform.SetParent(transform, false); // 'false' keeps the local scale and position
        }

        foreach (EnergyHandler energyHandler in transform.GetComponentsInChildren<EnergyHandler>())
        {
            energyHandler.updateView();
        }
    }
}
