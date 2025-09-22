using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

#nullable enable
public class CaptionEffect : GameEffect
{

    private string caption;
    private string? iconCaption;

    public CaptionEffect(string caption, string? iconCaption = null)
    {
        this.caption = caption;
        this.iconCaption = iconCaption;
    }

    public CaptionEffect(JObject json)
    {
        this.caption = LocalizationHelper.GetLocalizedString((JObject)json["caption"]!)!;
        this.iconCaption = json["iconCaption"] != null ?
            (json["iconCaption"]!.Type == JTokenType.Object ?
                LocalizationHelper.GetLocalizedString((JObject)json["iconCaption"]!) :
                json["iconCaption"]!.ToString()
            ) : null;
    }

    public override void applyEffect()
    {
        // This method is intentionally left empty
        // The caption effect does not have any gameplay effect
    }

    public override GameEffect Clone(Card? newOwner)
    {
        return new CaptionEffect(this.caption, this.iconCaption);
    }

    public override Task<string> getDescription()
    {
        return Task.FromResult(this.caption);
    }

    public override Task<string?> GetInternalIconDescription()
    {
        return Task.FromResult(this.iconCaption);
    }
}