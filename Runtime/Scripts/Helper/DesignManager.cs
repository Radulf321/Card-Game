using Newtonsoft.Json.Linq;
using UnityEngine;

#nullable enable

public class DesignManager
{
    private CardLayout cardLayout;
    private Color energyColor;
    private Color inactiveColor;

    public DesignManager(JObject? json)
    {
        this.cardLayout = new CardLayout(json?["cards"] as JObject);
        string energyColorString = json?["energy"]?["color"]?.ToString() ?? "ADE69F";
        UnityEngine.Debug.Log("Energy color string: " + energyColorString);
        ColorUtility.TryParseHtmlString("#" + energyColorString, out Color parsedEnergyColor);
        this.energyColor = parsedEnergyColor;
        string inactiveColorString = json?["inactiveColor"]?.ToString() ?? "999999";
        ColorUtility.TryParseHtmlString("#" + inactiveColorString, out Color parsedInactiveColor);
        this.inactiveColor = parsedInactiveColor;
    }

    public Color GetEnergyColor()
    {
        return energyColor;
    }

    public CardLayout GetCardLayout()
    {
        return cardLayout;
    }

    public Color GetInactiveColor()
    {
        return inactiveColor;
    }
}