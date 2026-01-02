#nullable enable

using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

public class DialogCondition : Dialog
{
    StatusCondition condition;
    Dialog? ifDialog;
    Dialog? elseDialog;

    public DialogCondition(StatusCondition condition, Dialog? ifDialog = null, Dialog? elseDialog = null, Dialog? nextDialog = null, string? id = null) : base(nextDialog: nextDialog, id: id)
    {
        this.condition = condition;
        this.ifDialog = ifDialog;
        this.elseDialog = elseDialog;
    }

    public DialogCondition(JObject json, Dialog? nextDialog = null) : this(condition: StatusCondition.FromJson(json["condition"] as JObject), ifDialog: Dialog.FromJson(json["if"]), elseDialog: Dialog.FromJson(json["else"]), nextDialog: nextDialog, id: GetIDFromJson(json))
    {
    }

    public override async Task ShowDialog()
    {
        Dialog? dialog;
        if (condition.IsFulfilled())
        {
            dialog = this.ifDialog;
        }
        else
        {
            dialog = this.elseDialog;
        }

        if (dialog != null)
        {
            await dialog.ShowDialog();
        }
        await base.ShowDialog();
    }

    public override List<Dialog> GetFollowingDialogs()
    {
        List<Dialog> result = base.GetFollowingDialogs();
        foreach (Dialog? dialog in new List<Dialog?>() { this.ifDialog, this.elseDialog })
        {
            if (dialog != null)
            {
                result.Add(dialog);
            }
        }
        return result;
    }
}