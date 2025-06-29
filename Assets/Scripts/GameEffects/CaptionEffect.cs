using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

public class CaptionEffect : GameEffect {

    private string caption;

    public CaptionEffect(string caption) {
        this.caption = caption;
    }

    public CaptionEffect(JObject json) {
        this.caption = LocalizationHelper.GetLocalizedString((JObject)json["caption"]);
    }

    public override void applyEffect() {
        // This method is intentionally left empty
        // The caption effect does not have any gameplay effect
    }

    public override GameEffect Clone(Card newOwner)
    {
        return new CaptionEffect(this.caption);
    }

    public override Task<string> getDescription() {
        return Task.FromResult(this.caption);
    }
}