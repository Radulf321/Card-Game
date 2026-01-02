using Newtonsoft.Json.Linq;

#nullable enable
public class Location : ActionCharacter {

    Dialog dialog;

    public Location(JObject json) : base(json) {
        this.dialog = Dialog.FromJson(json["dialog"] as JArray);
    }

    public override void ExecuteAction() {
        _ = DialogHandler.Instance!.StartDialog(this.dialog, onFinish: () =>
        {
            Game.Instance.EndRound();
        });
    }
}