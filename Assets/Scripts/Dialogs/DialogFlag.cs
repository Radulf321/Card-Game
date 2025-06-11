#nullable enable

using System.Threading.Tasks;

public class DialogFlag<T> : Dialog
{
    private FlagValidity validity;
    private string key;
    private T value;

    public DialogFlag(T value, string key, FlagValidity validity = FlagValidity.Game, Dialog? nextDialog = null) : base(nextDialog)
    {
        this.value = value;
        this.validity = validity;
        this.key = key;
    }

    public override async Task ShowDialog()
    {
        Game.Instance!.SetFlag<T>(validity, key, value);
        await base.ShowDialog();
    }
}