#nullable enable
using System;

public class EnumHelper
{
    public static T ParseEnum<T>(string? text)
    where T : struct
    {
        Enum.TryParse<T>(text, true, out T result);
        return result;
    }
}