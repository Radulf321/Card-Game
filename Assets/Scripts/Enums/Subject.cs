public enum Goal {
    Lust,
    SmallTalk,
}

public static class SubjectExtensions
{
    public static string GetName(this Goal subject)
    {
        switch (subject)
        {
            case Goal.Lust:
                return "Lust";
            case Goal.SmallTalk:
                return "Small Talk";
            default:
                return subject.ToString(); // Fallback to default enum name
        }
    }
}