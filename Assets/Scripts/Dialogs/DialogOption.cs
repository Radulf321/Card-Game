#nullable enable
using Newtonsoft.Json.Linq;

public class DialogOption
{
    private string title;
    private string? imagePath;
    private string? description;
    private Dialog? dialog;

    public DialogOption(string title, Dialog? dialog = null, string? description = null, string? imagePath = null, string? id = null)
    {
        this.title = title;
        this.description = description;
        this.dialog = dialog;
        this.imagePath = imagePath;
    }

    public DialogOption(JObject optionData)
    {
        this.title = LocalizationHelper.GetLocalizedString(optionData["title"] as JObject);
        this.description = (optionData["description"] != null) ? LocalizationHelper.GetLocalizedString(optionData["description"] as JObject) : null;
        switch (optionData?["dialog"]?.Type)
        {
            case JTokenType.Array:
                this.dialog = Dialog.FromJson(optionData["dialog"] as JArray);
                break;

            case JTokenType.Object:
                this.dialog = Dialog.FromJson((optionData["dialog"] as JObject)!);
                break;

            default:
                this.dialog = null;
                break;

        }
        this.imagePath = optionData?["image"]?.ToString();
    }

    public string? GetDescription()
    {
        return this.description;
    }

    public string GetTitle()
    {
        return this.title;
    }

    public string? GetImagePath()
    {
        if (this.imagePath == null)
        {
            return null;
        }
        return Game.Instance.GetResourcePath() + "/Graphics/Dialog/" + this.imagePath;
    }
    
    public Dialog? GetDialog()
    {
        return this.dialog;
    }
}