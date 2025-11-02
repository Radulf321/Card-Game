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
    private string gamePreparationBackground;
    private string equipmentBackground;
    private Color selectedColor;
    private List<SlotData> slots;
    private List<Equipment> unlockedEquipment;
    private List<Equipment> initialEquipment;
    private List<Equipment> selectedEquipment;
    private Action? onPreparationComplete;

    public EquipmentManager()
    {
        this.equipment = new Dictionary<string, Equipment>();
        this.gamePreparationBackground = "";
        this.equipmentBackground = "";
        this.selectedColor = Color.green;
        this.slots = new List<SlotData>();
        this.unlockedEquipment = new List<Equipment>();
        this.initialEquipment = new List<Equipment>();
        this.selectedEquipment = new List<Equipment>();
    }

    public EquipmentManager(String resourcePath, JObject json)
    {
        this.equipmentBackground = json["equipmentBackground"]!.ToString();
        this.gamePreparationBackground = json["gamePreparationBackground"]!.ToString();
        Dictionary<string, Equipment> equipment = new Dictionary<string, Equipment>();
        List<Equipment> initialEquipment = new List<Equipment>();
        TextAsset[] jsonFilesEquipment = Resources.LoadAll<TextAsset>(resourcePath + "/Equipment/");
        foreach (TextAsset jsonFile in jsonFilesEquipment)
        {
            JObject jsonObject = JObject.Parse(jsonFile.text);
            Equipment equipmentItem = new Equipment(jsonObject);
            equipment.Add(equipmentItem.GetID(), equipmentItem);
            if (equipmentItem.IsInitialEquipment())
            {
                initialEquipment.Add(equipmentItem);
            }
        }
        this.equipment = equipment;
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
        this.initialEquipment = initialEquipment;
        this.unlockedEquipment = new List<Equipment>();
        this.selectedEquipment = new List<Equipment>();
    }

    public void HandlePreparation(Action afterPreparation)
    {
        this.onPreparationComplete = afterPreparation;

        // Skip screen if no additional equipment is unlocked and
        // all starting equipment can be equipped simultaneously
        if (this.unlockedEquipment.Count == 0)
        {
            bool skipPreparation = true;
            List<string> takenSlots = new List<string>();
            foreach (Equipment equipment in this.initialEquipment)
            {
                string slot = equipment.GetSlot();
                if (!takenSlots.Contains(slot))
                {
                    takenSlots.Add(slot);
                }
                else
                {
                    skipPreparation = false;
                    break;
                }
            }
            if (skipPreparation)
            {
                this.ConfirmPreparation(this.initialEquipment);
                return;
            }
        }

        // Otherwise, load the game preparation scene
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
            if ((equipmentItem.GetSlot() == slot) && equipmentItem.IsAvailable())
            {
                equipmentList.Add(equipmentItem);
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
        // Store the selected equipment
        this.selectedEquipment = new List<Equipment>(selectedEquipment);
        Player player = Game.Instance.GetPlayer();
        player.AddSkill(Game.Instance.GetSkill("soManyIdeas"));

        foreach (Equipment equipmentItem in selectedEquipment)
        {
            equipmentItem.ApplyEffects(player);
        }
        this.onPreparationComplete?.Invoke();
        this.onPreparationComplete = null;
    }

    public List<Equipment> GetSelectedEquipment()
    {
        return new List<Equipment>(this.selectedEquipment);
    }

    public bool IsEquipmentUnlocked(string equipmentID)
    {
        return this.unlockedEquipment.Exists(e => e.GetID() == equipmentID);
    }

    public void UnlockEquipment(string equipmentID)
    {
        Equipment? equipment = GetEquipment(equipmentID);
        if (equipment == null)
        {
            throw new ArgumentException($"Equipment with ID {equipmentID} does not exist.");
        }

        if (!this.unlockedEquipment.Contains(equipment))
        {
            this.unlockedEquipment.Add(equipment);
        }
    }

    public void LoadFromJson(JObject json)
    {
        this.unlockedEquipment.Clear();
        this.selectedEquipment.Clear();

        foreach (string equipmentId in json["unlocked"]?.ToObject<List<string>>() ?? new List<string>())
        {
            Equipment? equipment = GetEquipment(equipmentId);
            if (equipment != null)
            {
                this.unlockedEquipment.Add(equipment);
            }
            else
            {
                Debug.LogWarning($"Equipment with ID {equipmentId} not found while loading unlocked equipment.");
            }
        }

        foreach (string equipmentId in json["selected"]?.ToObject<List<string>>() ?? new List<string>())
        {
            Equipment? equipment = GetEquipment(equipmentId);
            if (equipment != null)
            {
                this.selectedEquipment.Add(equipment);
            }
            else
            {
                Debug.LogWarning($"Equipment with ID {equipmentId} not found while loading selected equipment.");
            }
        }
    }
    
    public JObject SaveToJson()
    {
        JObject json = new JObject
        {
            ["unlocked"] = new JArray(this.unlockedEquipment.ConvertAll(equipment => equipment.GetID())),
            ["selected"] = new JArray(this.selectedEquipment.ConvertAll(equipment => equipment.GetID()))
        };
        return json;
    }
}