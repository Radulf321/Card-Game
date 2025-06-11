using System;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Unity.VisualScripting;

#nullable enable

public enum VerticalAlignment
{
    Top,
    Center,
    Bottom
}

public class DialogImage : Dialog
{
    private string? backgroundImagePath;
    private CharacterImageData leftCharacterImageData;
    private CharacterImageData rightCharacterImageData;

    public DialogImage(Dialog? nextDialog = null, string? backgroundImagePath = null, CharacterImageData? leftCharacterImageData = null, CharacterImageData? rightCharacterImageData = null) : base(nextDialog)
    {
        this.backgroundImagePath = backgroundImagePath;
        this.leftCharacterImageData = leftCharacterImageData ?? new CharacterImageData();
        this.rightCharacterImageData = rightCharacterImageData ?? new CharacterImageData();
    }

    public DialogImage(JObject dialogData, Dialog? nextDialog = null) : this(nextDialog, dialogData["background"]?.ToString(), new CharacterImageData(dialogData["left"]), new CharacterImageData(dialogData["right"])) { }

    public override async Task ShowDialog()
    {
        DialogHandler.Instance!.ShowImage(this);
        await base.ShowDialog();
    }

    public string? GetBackgroundImagePath()
    {
        if (backgroundImagePath == null)
        {
            return null;
        }
        if (backgroundImagePath == "")
        {
            return "";
        }
        return Game.Instance.GetResourcePath() + "/Graphics/Backgrounds/" + backgroundImagePath;
    }

    public CharacterImageData GetLeftCharacterImageData()
    {
        return leftCharacterImageData;
    }
    
    public CharacterImageData GetRightCharacterImageData()
    {
        return rightCharacterImageData;
    }
}


public class CharacterImageData
{
    private string? imagePath;
    private VerticalAlignment alignment;
    private bool mirror;

    public CharacterImageData(string? imagePath = null, VerticalAlignment alignment = VerticalAlignment.Bottom, bool mirror = false)
    {
        this.imagePath = imagePath;
        this.alignment = alignment;
        this.mirror = mirror;
    }

    public CharacterImageData(JToken? json)
    {
        switch (json?.Type ?? JTokenType.Null)
        {
            case JTokenType.Null:
                this.imagePath = null;
                this.alignment = VerticalAlignment.Bottom;
                this.mirror = false;
                break;

            case JTokenType.String:
                this.imagePath = json!.ToString();
                this.alignment = VerticalAlignment.Bottom;
                this.mirror = false;
                break;

            case JTokenType.Object:
                JObject jsonObject = (JObject)json!;
                this.imagePath = jsonObject["image"]?.ToString();
                this.alignment = EnumHelper.ParseEnum<VerticalAlignment>(jsonObject["alignment"]?.ToString()) ?? VerticalAlignment.Bottom;
                this.mirror = jsonObject["mirror"]?.ToObject<bool>() ?? false;
                break;

            default:
                throw new System.Exception("Invalid character image data type: " + json?.Type);
        }
    }

    public string? GetImagePath()
    {
        if (imagePath == null)
        {
            return null;
        }
        if (imagePath == "")
        {
            return "";
        }
        return Game.Instance.GetResourcePath() + "/Graphics/Characters/" + imagePath;
    }

    public VerticalAlignment GetAlignment()
    {
        return alignment;
    }
    
    public bool GetMirror()
    {
        return mirror;
    }
}