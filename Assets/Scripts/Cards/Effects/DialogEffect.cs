using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

public class DialogEffect : CardEffect
{
    private Dialog dialog;

    public DialogEffect(Dialog dialog)
    {
        this.dialog = dialog;
    }

    public DialogEffect(JObject json) : this(Dialog.FromJson(json["dialog"] as JArray))
    {
    }

    public override void applyEffect()
    {
        _ = DialogHandler.Instance.StartDialog(this.dialog, changeScene: false);
    }

    public override CardEffect Clone(Card newOwner)
    {
        return new DialogEffect(this.dialog);
    }

    public override Task<string> getDescription()
    {
        throw new System.NotImplementedException("DialogEffect does not have a description as it's not intended for card use.");
    }
}