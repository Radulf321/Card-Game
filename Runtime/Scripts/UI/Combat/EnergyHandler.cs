using UnityEngine;
using UnityEngine.UI;

public class EnergyHandler : MonoBehaviour, IViewUpdater
{

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void updateView() {
        Image image = GetComponent<Image>();
        Color color;
        int index = transform.GetSiblingIndex();
        if (index < CombatHandler.instance.getCurrentEnergy()) {
            color = Game.Instance.GetDesignManager().GetEnergyColor();
        } else {
            color = Game.Instance.GetDesignManager().GetInactiveColor();
        }

        if (index >= CombatHandler.instance.getMaxEnergy()) {
            color.a = 0.4f;
        }

        image.color = color;
    }
}
