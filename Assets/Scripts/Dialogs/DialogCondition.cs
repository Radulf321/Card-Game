#nullable enable

using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

public class DialogCondition : Dialog
{
    StatusCondition condition;
    Dialog? ifDialog;
    Dialog? elseDialog;

    public DialogCondition(StatusCondition condition, Dialog? ifDialog = null, Dialog? elseDialog = null, Dialog? nextDialog = null) : base(nextDialog)
    {
        this.condition = condition;
        this.ifDialog = ifDialog;
        this.elseDialog = elseDialog;
    }

    public DialogCondition(JObject json, Dialog? nextDialog = null) : this(condition: StatusCondition.FromJson(json["condition"] as JObject), ifDialog: Dialog.FromJson(json["if"]), elseDialog: Dialog.FromJson(json["else"]), nextDialog: nextDialog)
    {
    }

    public override async Task ShowDialog()
    {
        Dialog? dialog;
        if (condition.IsFulfilled())
        {
            UnityEngine.Debug.Log("Show If Dialog");
            dialog = this.ifDialog;
        }
        else
        {
            UnityEngine.Debug.Log("Show Else Dialog");
            dialog = this.elseDialog;
        }

        if (dialog != null)
        {
            await dialog.ShowDialog();
        }
        UnityEngine.Debug.Log("Show Next Dialog");
        await base.ShowDialog();
    }
}