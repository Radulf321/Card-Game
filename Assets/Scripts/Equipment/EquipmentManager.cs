using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;

#nullable enable

public class SlotData
{
    private string id;
    private float x;
    private float y;
    private float width;
    private float height;

    public SlotData(string id, float x, float y, float width, float height)
    {
        this.id = id;
        this.x = x;
        this.y = y;
        this.width = width;
        this.height = height;
    }

    public SlotData(JObject json)
    {
        this.id = json["id"]!.ToString();
        this.x = json["x"]!.ToObject<float>();
        this.y = json["y"]!.ToObject<float>();
        this.width = json["width"]!.ToObject<float>();
        this.height = json["height"]!.ToObject<float>();
    }

    public string GetID()
    {
        return this.id;
    }

    public float GetX()
    {
        return this.x;
    }

    public float GetY()
    {
        return this.y;
    }

    public float GetWidth()
    {
        return this.width;
    }

    public float GetHeight()
    {
        return this.height;
    }
}
public class EquipmentManager
{
    private Dictionary<string, Equipment> equipment;
    private List<string> startingEquipment;
    private string gamePreparationBackground;
    private string equipmentBackground;
    private Color selectedColor;
    private List<SlotData> slots;
    private Action? onPreparationComplete;

    public EquipmentManager()
    {
        this.equipment = new Dictionary<string, Equipment>();
        this.startingEquipment = new List<string>();
        this.gamePreparationBackground = "";
        this.equipmentBackground = "";
        this.selectedColor = Color.green;
        this.slots = new List<SlotData>();
    }

    public EquipmentManager(String resourcePath, JObject json)
    {
        this.equipmentBackground = json["equipmentBackground"]!.ToString();
        this.gamePreparationBackground = json["gamePreparationBackground"]!.ToString();
        Dictionary<string, Equipment> equipment = new Dictionary<string, Equipment>();
        TextAsset[] jsonFilesEquipment = Resources.LoadAll<TextAsset>(resourcePath + "/Equipment/");
        foreach (TextAsset jsonFile in jsonFilesEquipment)
        {
            JObject jsonObject = JObject.Parse(jsonFile.text);
            Equipment equipmentItem = new Equipment(jsonObject);
            equipment.Add(equipmentItem.GetID(), equipmentItem);
        }
        this.equipment = equipment;
        this.startingEquipment = json["startingEquipment"]!.ToObject<List<string>>() ?? new List<string>();
        int rgb = json["selectedColor"]!.ToObject<int>(); // Default to green if not specified
        float r = ((rgb >> 16) & 0xFF) / 255f;
        float g = ((rgb >> 8) & 0xFF) / 255f;
        float b = (rgb & 0xFF) / 255f;
        this.selectedColor = new Color(r, g, b);
        List<SlotData> slots = new List<SlotData>();
        foreach (JObject slotJson in json["slots"] as JArray ?? new JArray())
        {
            SlotData slotData = new SlotData(slotJson);
            slots.Add(slotData);
        }
        this.slots = slots;
    }

    public void HandlePreparation(Action afterPreparation)
    {
        this.onPreparationComplete = afterPreparation;
        // TODO: Check if equipment screen should be shown and if so, show it
        // Skip screen if no additional equipment is unlocked and
        // all starting equipment can be equipped simultaneously
        
        FadeHandler.Instance!.LoadScene("GamePreparationScene");
    }

    public string GetEquipmentBackground()
    {
        return Game.Instance.GetResourcePath() + "/Graphics/Backgrounds/" + this.equipmentBackground;
    }

    public string GetGamePreparationBackground()
    {
        return Game.Instance.GetResourcePath() + "/Graphics/Backgrounds/" + this.gamePreparationBackground;
    }

    public Equipment? GetEquipment(string id)
    {
        if (this.equipment.TryGetValue(id, out Equipment? equipmentItem))
        {
            return equipmentItem;
        }
        return null;
    }

    public List<Equipment> GetEquipmentForSlot(string slot)
    {
        List<Equipment> equipmentList = new List<Equipment>();
        foreach (Equipment equipmentItem in this.equipment.Values)
        {
            if (equipmentItem.GetSlot() == slot)
            {
                equipmentList.Add(equipmentItem);
                UnityEngine.Debug.Log($"Found equipment for slot {slot}: {equipmentItem.GetID()}");
            }
        }
        return equipmentList;
    }

    public Color GetSelectedColor()
    {
        return this.selectedColor;
    }

    public List<SlotData> GetSlots()
    {
        return this.slots;
    }

    public void ConfirmPreparation(List<Equipment> selectedEquipment)
    {
        foreach (Equipment equipmentItem in selectedEquipment)
        {
            equipmentItem.ApplyEffects(Game.Instance.GetPlayer());
        }
        this.onPreparationComplete?.Invoke();
        this.onPreparationComplete = null;
    }
}