using Newtonsoft.Json.Linq;
using UnityEngine;

#nullable enable

public class CardLayout
{
    private Sprite? backgroundImage;
    private Sprite? descriptionBackgroundImage;
    private float? descriptionBackgroundLeft;
    private float? descriptionBackgroundRight;
    private RectOffset descriptionPadding;
    private Color? titleGradientLeft;
    private Color? titleGradientRight;
    private RectOffset energyPadding;

    public CardLayout(JObject? json)
    {
        if (json?["background"] != null)
        {
            string backgroundName = json["background"]!.ToString();
            backgroundImage = Resources.Load<Sprite>("CardData/Graphics/" + backgroundName);
            if (backgroundImage == null)
            {
                Debug.LogWarning("Could not load card background image: " + backgroundName);
            }
        }
        if (json?["descriptionBackground"] != null)
        {
            string? backgroundName = json["descriptionBackground"]!["image"]?.ToString();
            if (backgroundName != null) {
                descriptionBackgroundImage = Resources.Load<Sprite>("CardData/Graphics/" + backgroundName);
                if (descriptionBackgroundImage == null)
                {
                    Debug.LogWarning("Could not load card description background image: " + backgroundName);
                }
            }
            float? left = json["descriptionBackground"]!["left"]?.ToObject<float>();
            if (left != null)
            {
                descriptionBackgroundLeft = left;
            }
            float? right = json["descriptionBackground"]!["right"]?.ToObject<float>();
            if (right != null)
            {
                descriptionBackgroundRight = right;
            }
        }
        JObject? descriptionPadding = json?["descriptionPadding"] as JObject;
        this.descriptionPadding = new RectOffset(
            descriptionPadding?["left"]?.ToObject<int>() ?? 52,
            descriptionPadding?["right"]?.ToObject<int>() ?? 59,
            descriptionPadding?["top"]?.ToObject<int>() ?? 30,
            descriptionPadding?["bottom"]?.ToObject<int>() ?? 30
        );
        JObject? energyPadding = json?["energyPadding"] as JObject;
        this.energyPadding = new RectOffset(
            energyPadding?["left"]?.ToObject<int>() ?? 0,
            energyPadding?["right"]?.ToObject<int>() ?? 0,
            energyPadding?["top"]?.ToObject<int>() ?? 0,
            energyPadding?["bottom"]?.ToObject<int>() ?? 0
        );
        JObject? titleGradient = json?["title"]?["gradient"] as JObject;
        if (titleGradient != null)
        {
            string? leftColor = titleGradient?["left"]?.ToString();
            if (leftColor != null)
            {
                ColorUtility.TryParseHtmlString("#" + leftColor, out Color parsedLeftColor);
                titleGradientLeft = parsedLeftColor;
            }
            string? rightColor = titleGradient?["right"]?.ToString();
            if (rightColor != null)
            {
                ColorUtility.TryParseHtmlString("#" + rightColor, out Color parsedRightColor);
                titleGradientRight = parsedRightColor;
            }
        }
    }

    public Sprite? GetBackgroundImage()
    {
        return backgroundImage;
    }

    public Sprite? GetDescriptionBackgroundImage()
    {
        return descriptionBackgroundImage;
    }

    public float? GetDescriptionBackgroundLeft()
    {
        return descriptionBackgroundLeft;
    }

    public float? GetDescriptionBackgroundRight()
    {
        return descriptionBackgroundRight;
    }

    public RectOffset GetDescriptionPadding()
    {
        return descriptionPadding;
    }

    public RectOffset GetEnergyPadding()
    {
        return energyPadding;
    }

    public Color? GetTitleGradientLeft()
    {
        return titleGradientLeft;
    }

    public Color? GetTitleGradientRight()
    {
        return titleGradientRight;
    }
}