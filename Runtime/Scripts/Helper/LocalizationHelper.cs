using Newtonsoft.Json.Linq;

#nullable enable
public class LocalizationHelper
{
    private static string defaultLocalization = "en";
    public static string? GetLocalizedString(JObject? json)
    {
        if (json == null)
        {
            return null;
        }
        if (json.ContainsKey(GetLocalization()))
        {
            return json[GetLocalization()]!.ToString();
        }
        else if (json.ContainsKey(defaultLocalization))
        {
            return json[defaultLocalization]!.ToString();
        }
        else
        {
            throw new System.Exception("Invalid localization object: " + json.ToString());
        }
    }

    public static string GetLocalization()
    {
        return "de";
    }
}