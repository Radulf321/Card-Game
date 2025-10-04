using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using UnityEngine.Localization.Settings;

#nullable enable

public class EquipmentTask : GameTask
{
    private List<string> equipmentIds;

    public EquipmentTask(string id, List<Reward> rewards, List<string> equipmentIds) : base(id, rewards)
    {
        this.equipmentIds = equipmentIds;
    }

    public EquipmentTask(JObject json) : base(json)
    {
        this.equipmentIds = json["equipment"]?.ToObject<List<string>>() ?? new List<string>();
    }

    public override Task<string> GetDescription()
    {
        List<string> names = new List<string>();
        foreach (string equipmentId in equipmentIds)
        {
            Equipment? equipment = Game.Instance.GetEquipmentManager().GetEquipment(equipmentId);
            if (equipment != null)
            {
                names.Add(equipment.GetName());
            }
            else
            {
                names.Add("##Invalid Equipment##");
            }
        }
        return AsyncHelper.HandleToTask(LocalizationSettings.StringDatabase.GetLocalizedStringAsync("TaskStrings", "EquipmentDescription", arguments: new Dictionary<string, object?> {
            { "equipment", names }
        }));
    }

    public override bool IsCompleted()
    {
        List<Equipment> selectedEquipment = Game.Instance.GetEquipmentManager().GetSelectedEquipment();
        List<string> selectedEquipmentIds = new List<string>();

        foreach (Equipment equipment in selectedEquipment)
        {
            selectedEquipmentIds.Add(equipment.GetID());
        }

        // Check if all required equipment IDs are in the selected equipment
        foreach (string requiredId in equipmentIds)
        {
            if (!selectedEquipmentIds.Contains(requiredId))
            {
                return false;
            }
        }

        return true;
    }
}