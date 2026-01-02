#nullable enable

using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

public class DialogJump : Dialog
{
    string target;

    public DialogJump(string target, Dialog? nextDialog = null, string? id = null) : base(nextDialog: nextDialog, id: id)
    {
        this.target = target;
    }

    public DialogJump(JObject json, Dialog? nextDialog = null) : this(target: json["target"]!.ToString(), nextDialog: nextDialog, id: Dialog.GetIDFromJson(json))
    {
    }

    public override async Task ShowDialog()
    {

        Dialog? targetDialog = Dialog.GetDialogByID(this.target);
        if (targetDialog != null)
        {
            await targetDialog.ShowDialog();
        }
        await base.ShowDialog();
    }
}