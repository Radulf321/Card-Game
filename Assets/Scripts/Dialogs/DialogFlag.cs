#nullable enable

using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

public enum FlagOperation
{
    Set,
    Increment
}

public class DialogFlag : Dialog
{
    private FlagValidity validity;
    private string key;
    private object value;
    private FlagOperation operation;

    public DialogFlag(object value, string key, FlagOperation operation = FlagOperation.Set, FlagValidity validity = FlagValidity.Game, Dialog? nextDialog = null, string? id = null) : base(nextDialog: nextDialog, id: id)
    {
        if (value is long longValue)
        {
            value = (int)longValue;
        }
        if (!(value is bool || value is int || value is float || value is string))
        {
            throw new System.ArgumentException("Value must be bool, int, float, or string");
        }
        this.value = value;
        this.validity = validity;
        this.key = key;
        this.operation = operation;
    }

    public DialogFlag(JObject json, Dialog? nextDialog = null) : this(value: json["value"]!.ToObject<object>()!, operation: EnumHelper.ParseEnum<FlagOperation>(json["operation"]?.ToString()) ?? FlagOperation.Set, validity: EnumHelper.ParseEnum<FlagValidity>(json["validity"]?.ToString()) ?? FlagValidity.Game, key: json["key"]?.ToString()!, nextDialog: nextDialog, id: GetIDFromJson(json))
    {
    }

    public override async Task ShowDialog()
    {
        object value;
        switch (this.operation)
        {
            case FlagOperation.Set:
                value = this.value;
                break;

            case FlagOperation.Increment:
                object current = Game.Instance!.GetFlag(this.validity, this.key) ?? 0;
                if ((current is int || current is float) && (this.value is int || this.value is float))
                {
                    // Convert both to float for calculation, then decide result type
                    float currentFloat = current is int currentInt ? currentInt : (float)current;
                    float valueFloat = this.value is int valueInt ? valueInt : (float)this.value;
                    float result = currentFloat + valueFloat;

                    // Keep as int if both original values were int, otherwise use float
                    value = (current is int && this.value is int) ? (object)(int)result : result;
                }
                else
                {
                    throw new System.InvalidOperationException("Cannot increment non-numeric flag");
                }
                break;

            default:
                throw new System.ArgumentOutOfRangeException();
        }
        Game.Instance!.SetFlag(validity, key, value);
        await base.ShowDialog();
    }
}