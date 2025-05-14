using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

#nullable enable
public class DialogImage : Dialog
{
    private string? backgroundImagePath;
    private string? leftCharacterImagePath;
    private string? rightCharacterImagePath;

    private Action onFinish;

    public DialogImage(Action onFinish, string? backgroundImagePath = null, string? leftCharacterImagePath = null, string? rightCharacterImagePath = null)
    {
        this.backgroundImagePath = backgroundImagePath;
        this.leftCharacterImagePath = leftCharacterImagePath;
        this.rightCharacterImagePath = rightCharacterImagePath;
        this.onFinish = onFinish;
    }

    public DialogImage(Dialog nextDialog, string? backgroundImagePath = null, string? leftCharacterImagePath = null, string? rightCharacterImagePath = null) : this(nextDialog.ShowDialog, backgroundImagePath, leftCharacterImagePath, rightCharacterImagePath) { }

    public DialogImage(JObject dialogData, Action onFinish) : this(onFinish, dialogData["background"]?.ToString(), dialogData["left"]?.ToString(), dialogData["right"]?.ToString()) { }

    public Action GetOnFinish()
    {
        return onFinish;
    }

    public override void ShowDialog()
    {
        UnityEngine.Object.FindAnyObjectByType<DialogHandler>().ShowImage(this);
    }

    public string? GetBackgroundImagePath()
    {
        return Game.Instance.GetResourcePath() + "/Graphics/Backgrounds/" + backgroundImagePath;
    }

    public string? GetLeftCharacterImagePath()
    {
        if (leftCharacterImagePath == null)
        {
            return null;
        }
        if (leftCharacterImagePath == "")
        {
            return "";
        }
        return Game.Instance.GetResourcePath() + "/Graphics/Characters/" + leftCharacterImagePath;
    }

    public string? GetRightCharacterImagePath()
    {
        if (rightCharacterImagePath == null)
        {
            return null;
        }
        if (rightCharacterImagePath == "")
        {
            return "";
        }
        return Game.Instance.GetResourcePath() + "/Graphics/Characters/" + rightCharacterImagePath;
    }
}