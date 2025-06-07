using Newtonsoft.Json.Linq;

#nullable enable
public class Location : ActionCharacter {

    Dialog dialog;

    public Location(JObject json) : base(json) {
        this.dialog = Dialog.FromJson(json["dialog"] as JArray, onFinish: Game.Instance.EndRound);
    }

    protected override void ExecuteAction() {
        DialogHandler.Instance!.StartDialog(this.dialog);
    }
}