#nullable enable

using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

public class DialogRandom : Dialog
{
    List<Dialog> dialogs;

    public DialogRandom(List<Dialog> dialogs, Dialog? nextDialog = null, string? id = null) : base(nextDialog: nextDialog, id: id)
    {
        this.dialogs = dialogs;
    }

    public DialogRandom(JObject json, Dialog? nextDialog = null) : base(nextDialog: nextDialog, id: Dialog.GetIDFromJson(json))
    {
        List<Dialog> dialogs = new List<Dialog>();
        foreach (JToken dialogData in json["dialogs"] as JArray ?? new JArray())
        {
            Dialog? dialog = Dialog.FromJson(dialogData);
            if (dialog != null)
            {
                dialogs.Add(dialog);
            }
        }
        this.dialogs = dialogs;
    }

    public override async Task ShowDialog()
    {
        if (this.dialogs.Count > 0)
        {
            int index = UnityEngine.Random.Range(0, this.dialogs.Count);
            await this.dialogs[index].ShowDialog();
        }
        await base.ShowDialog();
    }

    public override List<Dialog> GetFollowingDialogs()
    {
        List<Dialog> result = base.GetFollowingDialogs();
        result.AddRange(this.dialogs);
        return result;
    }
}