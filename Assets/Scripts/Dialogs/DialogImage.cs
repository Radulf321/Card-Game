using System;
using Newtonsoft.Json.Linq;

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

    private Action onFinish;

    public DialogImage(Action onFinish, string? backgroundImagePath = null, CharacterImageData? leftCharacterImageData = null, CharacterImageData? rightCharacterImageData = null)
    {
        this.backgroundImagePath = backgroundImagePath;
        this.leftCharacterImageData = leftCharacterImageData ?? new CharacterImageData();
        this.rightCharacterImageData = rightCharacterImageData ?? new CharacterImageData();
        this.onFinish = onFinish;
    }

    public DialogImage(Action onFinish, string? backgroundImagePath = null, string? leftCharacterImagePath = null, string? rightCharacterImagePath = null)
    {
        this.backgroundImagePath = backgroundImagePath;
        this.leftCharacterImageData = new CharacterImageData(leftCharacterImagePath);
        this.rightCharacterImageData = new CharacterImageData(rightCharacterImagePath);
        this.onFinish = onFinish;
    }

    public DialogImage(Dialog nextDialog, string? backgroundImagePath = null, string? leftCharacterImagePath = null, string? rightCharacterImagePath = null) : this(nextDialog.ShowDialog, backgroundImagePath, leftCharacterImagePath, rightCharacterImagePath) { }

    public DialogImage(JObject dialogData, Action onFinish) : this(onFinish, dialogData["background"]?.ToString(), new CharacterImageData(dialogData["left"]), new CharacterImageData(dialogData["right"])) { }

    public Action GetOnFinish()
    {
        return onFinish;
    }

    public override void ShowDialog()
    {
        DialogHandler.Instance!.ShowImage(this);
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