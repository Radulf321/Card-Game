using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using UnityEngine;

#nullable enable
public class CharacterManager
{
    private List<CombatTarget> combatTargets;
    private List<Location> locations;
    private List<ActionCharacter> unlockedCharacters;

    public CharacterManager()
    {
        this.combatTargets = new List<CombatTarget>();
        this.locations = new List<Location>();
        this.unlockedCharacters = new List<ActionCharacter>();
    }

    public CharacterManager(string ResourcePath)
    {
        List<CombatTarget> combatTargets = new List<CombatTarget>();
        TextAsset[] jsonFilesCombatTarget = Resources.LoadAll<TextAsset>(ResourcePath + "/CombatTargets/");
        foreach (TextAsset jsonFile in jsonFilesCombatTarget)
        {
            JObject jsonObject = JObject.Parse(jsonFile.text);
            CombatTarget combatTarget = new CombatTarget(jsonObject);
            combatTargets.Add(combatTarget);
        }
        this.combatTargets = combatTargets;
        List<Location> locations = new List<Location>();
        TextAsset[] jsonFilesLocations = Resources.LoadAll<TextAsset>(ResourcePath + "/Locations/");
        foreach (TextAsset jsonFile in jsonFilesLocations)
        {
            JObject jsonObject = JObject.Parse(jsonFile.text);
            Location location = new Location(jsonObject);
            locations.Add(location);
        }
        this.locations = locations;
        this.unlockedCharacters = new List<ActionCharacter>();
    }

    public CombatTarget? GetTutorialTarget()
    {
        return this.combatTargets.Find(target => target.GetTargetType() == TargetType.Tutorial);
    }

    public List<DialogOption> GetRoundOptions()
    {
        List<DialogOption> options = new List<DialogOption>();
        List<List<DialogOption>> dialogOptionsByType = new List<List<DialogOption>>();
        foreach (IEnumerable<ActionCharacter> actionCharacters in new List<IEnumerable<ActionCharacter>>() { GetAvailableCombatTargets().Cast<ActionCharacter>(), GetAvailableLocations().Cast<ActionCharacter>() })
        {
            List<DialogOption> dialogOptionsForType = new List<DialogOption>();
            foreach (ActionCharacter actionCharacter in actionCharacters)
            {
                DialogOption? dialogOption = actionCharacter.GetDialogOption();
                if (dialogOption != null)
                {
                    dialogOptionsForType.Add(dialogOption);
                }
            }
            if (dialogOptionsForType.Count > 0)
            {
                int index = UnityEngine.Random.Range(0, dialogOptionsForType.Count);
                options.Add(dialogOptionsForType[index]);
                dialogOptionsForType.RemoveAt(index);
            }
            if (dialogOptionsForType.Count > 0)
            {
                dialogOptionsByType.Add(dialogOptionsForType);
            }
        }

        if (dialogOptionsByType.Count > 0)
        {
            int typeIndex = UnityEngine.Random.Range(0, dialogOptionsByType.Count);
            int optionIndex = UnityEngine.Random.Range(0, dialogOptionsByType[typeIndex].Count);
            options.Insert(1, dialogOptionsByType[typeIndex][optionIndex]);
        }

        return options;
    }

    public CombatTarget? GetCombatTarget(string? id)
    {
        foreach (CombatTarget target in combatTargets)
        {
            if (target.GetID() == id)
            {
                return target;
            }
        }
        return null;
    }

    public Location? GetLocation(string id)
    {
        foreach (Location location in locations)
        {
            if (location.GetID() == id)
            {
                return location;
            }
        }
        return null;
    }

    public ActionCharacter? GetActionCharacter(string id)
    {
        CombatTarget? combatTarget = GetCombatTarget(id);
        if (combatTarget != null)
        {
            return combatTarget;
        }
        else
        {
            return GetLocation(id);
        }
    }

    public bool IsCharacterUnlocked(string characterID)
    {
        return this.unlockedCharacters.Exists(e => e.GetID() == characterID);
    }

    public void UnlockCharacter(string characterID)
    {
        ActionCharacter? character = GetActionCharacter(characterID);
        if (character == null)
        {
            throw new System.ArgumentException($"Character with ID {characterID} does not exist.");
        }

        if (!this.unlockedCharacters.Contains(character))
        {
            this.unlockedCharacters.Add(character);
        }
    }

    public JObject SaveToJson()
    {
        JObject json = new JObject
        {
            ["unlocked"] = new JArray(this.unlockedCharacters.Select(character => character.GetID()))
        };
        return json;
    }

    public void LoadFromJson(JObject json)
    {
        this.unlockedCharacters.Clear();

        foreach (string characterId in json["unlocked"]?.ToObject<List<string>>() ?? new List<string>())
        {
            UnlockCharacter(characterId);
        }
    }

    public List<CombatTarget> GetAvailableCombatTargets()
    {
        return this.combatTargets.Where(combatTarget => combatTarget.IsAvailable()).ToList();
    }

    private List<Location> GetAvailableLocations()
    {
        return this.locations.Where(location => location.IsAvailable()).ToList();
    }
}