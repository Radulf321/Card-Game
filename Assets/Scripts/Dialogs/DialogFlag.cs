#nullable enable

using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

public class DialogFlag : Dialog
{
    private FlagValidity validity;
    private string key;
    private object value;

    public DialogFlag(object value, string key, FlagValidity validity = FlagValidity.Game, Dialog? nextDialog = null, string? id = null) : base(nextDialog: nextDialog, id: id)
    {
        if (!(value is bool || value is int || value is float || value is string))
        {
            throw new System.ArgumentException("Value must be bool, int, float, or string");
        }
        this.value = value;
        this.validity = validity;
        this.key = key;
    }

    public DialogFlag(JObject json, Dialog? nextDialog = null) : this(value: json["value"]!.ToObject<object>()!, validity: EnumHelper.ParseEnum<FlagValidity>(json["validity"]?.ToString()) ?? FlagValidity.Game, key: json["key"]?.ToString()!, nextDialog: nextDialog, id: GetIDFromJson(json))
    {
    }

    public override async Task ShowDialog()
    {
        Game.Instance!.SetFlag(validity, key, value);
        await base.ShowDialog();
    }
}