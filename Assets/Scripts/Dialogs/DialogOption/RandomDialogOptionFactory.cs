#nullable enable

using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Unity.Mathematics;

public class RandomDialogOptionFactory : DialogOptionFactory
{
    private List<DialogOptionFactory> options;
    private int amount;

    public RandomDialogOptionFactory(List<DialogOptionFactory> options, int amount = 1)
    {
        this.options = options;
        this.amount = amount;
    }

    public RandomDialogOptionFactory(JObject json) : this(options: DialogOptionFactory.FromJson(json["options"]!), amount: json["amount"]?.ToObject<int>() ?? 1)
    {
    }

    override public List<DialogOption> GetOptions(bool allOptions = false)
    {
        if (allOptions)
        {
            return JoinFactories(options, allOptions);
        }
        else
        {
            List<int> indexes = new List<int>();
            List<DialogOption> result = new List<DialogOption>();
            while (indexes.Count < math.min(this.amount, options.Count))
            {
                int newIndex = UnityEngine.Random.Range(0, options.Count);
                if (!indexes.Contains(newIndex))
                {
                    indexes.Add(newIndex);
                    result.AddRange(this.options[newIndex].GetOptions(allOptions));
                }
            }

            return result;
        }
    }
}