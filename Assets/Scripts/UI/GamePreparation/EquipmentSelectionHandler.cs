using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

#nullable enable
public class EquipmentSelectionHandler : MonoBehaviour
{
    public GameObject equipmentBoxPrefab;
    private string? slot;
    private Equipment? selectedEquipment;
    private Dictionary<string, GameObject> equipmentBoxes = new Dictionary<string, GameObject>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        List<Equipment> equipmentList = Game.Instance.GetEquipmentManager().GetEquipmentForSlot(slot);
        Transform boxes = transform.Find("EquipmentBoxes");
        for (int i = 0; i < boxes.childCount; i++)
        {
            Destroy(boxes.GetChild(i).gameObject);
        }
        equipmentBoxes.Clear();
        foreach (Equipment equipment in equipmentList)
        {
            GameObject box = Instantiate(equipmentBoxPrefab, boxes);
            EquipmentBoxHandler boxHandler = box.GetComponent<EquipmentBoxHandler>();
            boxHandler.SetEquipment(equipment);
            boxHandler.SetOnClickAction(SetSelectedEquipment);
            equipmentBoxes[equipment.GetID()] = box;
        }
        if (equipmentList.Count > 0)
        {
            SetSelectedEquipment(Game.Instance.GetEquipmentManager().GetEquipment(equipmentList[0].GetID()));
        }
        else
        {
            SetSelectedEquipment(null);
        }
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void SetSlot(string? slot)
    {
        this.slot = slot;
        List<Equipment> equipmentList = (slot == null) ? new List<Equipment>() : Game.Instance.GetEquipmentManager().GetEquipmentForSlot(slot);
        Transform boxes = transform.Find("EquipmentBoxes");
        for (int i = 0; i < boxes.childCount; i++)
        {
            Destroy(boxes.GetChild(i).gameObject);
        }
        equipmentBoxes.Clear();
        foreach (Equipment equipment in equipmentList)
        {
            GameObject box = Instantiate(equipmentBoxPrefab, boxes);
            EquipmentBoxHandler boxHandler = box.GetComponent<EquipmentBoxHandler>();
            boxHandler.SetEquipment(equipment);
            boxHandler.SetOnClickAction(SetSelectedEquipment);
            equipmentBoxes[equipment.GetID()] = box;
        }
        // Confirm selected equipment again, assuring the corresponding box is selected
        Equipment? previousSelected = selectedEquipment;
        SetSelectedEquipment(null);
        SetSelectedEquipment(previousSelected);
    }

    public Equipment? GetSelectedEquipment()
    {
        return selectedEquipment;
    }

    private void SetSelectedEquipment(Equipment? equipment)
    {
        if (this.selectedEquipment == equipment)
        {
            return;
        }
        if (this.selectedEquipment != null && equipmentBoxes.ContainsKey(this.selectedEquipment.GetID()))
        {
            equipmentBoxes[this.selectedEquipment.GetID()].GetComponent<EquipmentBoxHandler>().SetSelected(false);
        }
        this.selectedEquipment = equipment;
        if (equipment != null && equipmentBoxes.ContainsKey(equipment.GetID()))
        {
            equipmentBoxes[equipment.GetID()].GetComponent<EquipmentBoxHandler>().SetSelected(true);
        }
        transform.Find("CardDisplay").GetComponent<CardListDisplay>().SetCards(equipment?.GetCards() ?? new List<Card>());
        _ = UpdateText();
    }

    private async Task UpdateText() {
        TextMeshProUGUI effectText = transform.Find("OtherEffectDisplay").GetComponent<TextMeshProUGUI>();
        effectText.text = "";
        if (selectedEquipment != null)
        {
            string otherEffectText = await selectedEquipment.GetOtherEffectText();
            effectText.text = otherEffectText;
        }
    }
}
