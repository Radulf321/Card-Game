using System.Collections.Generic;
using Newtonsoft.Json.Linq;

public class EnergyInfo
{
    private Dictionary<int, int> energy;

    public EnergyInfo(Dictionary<int, int> energy = null)
    {
        this.energy = energy ?? new Dictionary<int, int>() { { 0, 1 } };
    }

    public EnergyInfo(JObject json)
    {
        Dictionary<int, int> energy = new Dictionary<int, int>();
        foreach (JProperty property in json.Properties())
        {
            energy.Add(int.Parse(property.Name), property.Value.ToObject<int>());
        }
        this.energy = energy;
    }

    public int GetStartingEnergy()
    {
        return GetEnergyForTurn(0);
    }

    public int GetEnergyForTurn(int turn)
    {
        return Game.Instance.ApplyModifiers(ModifierType.Energy, energy.TryGetValue(turn, out int value) ? value : 0, turn);
    }

    public void AddEnergy(int amount, int turn)
    {
        if (this.energy.ContainsKey(turn))
        {
            energy[turn] += amount;
        }
        else
        {
            energy[turn] = amount;
        }
    }

    public JObject SaveToJson()
    {
        JObject saveData = new JObject();
        foreach (KeyValuePair<int, int> pair in energy)
        {
            saveData[pair.Key.ToString()] = pair.Value;
        }
        return saveData;
    }
}