using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

#nullable enable
public class GamePreparationHandler : MonoBehaviour
{
    private List<EquipmentSelectionHandler> equipmentSelectionHandlers = new List<EquipmentSelectionHandler>();
    public GameObject equipmentSelectionPrefab;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        EquipmentManager equipmentManager = Game.Instance.GetEquipmentManager();
        transform.Find("Background").GetComponent<Image>().sprite = Resources.Load<Sprite>(equipmentManager.GetGamePreparationBackground());
        Transform equipmentArea = transform.Find("EquipmentContainer").Find("EquipmentArea");
        equipmentArea.GetComponent<Image>().sprite = Resources.Load<Sprite>(equipmentManager.GetEquipmentBackground());
        Transform equipmentSelectionArea = equipmentArea.Find("EquipmentSelectionArea");
        for (int i = 0; i < equipmentSelectionArea.childCount; i++)
        {
            Destroy(equipmentSelectionArea.GetChild(i).gameObject);
        }
        equipmentSelectionHandlers.Clear();
        foreach (SlotData slot in equipmentManager.GetSlots())
        {
            GameObject equipmentSelection = Instantiate(equipmentSelectionPrefab, equipmentSelectionArea);
            EquipmentSelectionHandler selectionHandler = equipmentSelection.GetComponent<EquipmentSelectionHandler>();
            selectionHandler.SetSlot(slot.GetID());
            RectTransform selectionRect = equipmentSelection.GetComponent<RectTransform>();
            selectionRect.anchorMin = new Vector2(slot.GetX(), slot.GetY());
            selectionRect.anchorMax = new Vector2(slot.GetX() + slot.GetWidth(), slot.GetY() + slot.GetHeight());
            equipmentSelectionHandlers.Add(selectionHandler);
        }
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void ConfirmPreparation()
    {
        List<Equipment> selectedEquipment = new List<Equipment>();
        foreach (EquipmentSelectionHandler handler in equipmentSelectionHandlers)
        {
            Equipment? equipment = handler.GetSelectedEquipment();
            if (equipment != null)
            {
                selectedEquipment.Add(equipment);
            }
        }
        Game.Instance.GetEquipmentManager().ConfirmPreparation(selectedEquipment);
    }
}
