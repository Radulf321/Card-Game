#nullable enable
using System;

public class EnumHelper
{
    public static T? ParseEnum<T>(string? text)
    where T : struct
    {
        bool success = Enum.TryParse<T>(text, true, out T result);
        if (!success)
        {
            return null;
        }
        else
        {
            return result;
        }
    }
}