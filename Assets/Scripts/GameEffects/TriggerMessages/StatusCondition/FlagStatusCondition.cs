using Newtonsoft.Json.Linq;

public class FlagStatusCondition : StatusCondition
{
    private object value;
    private string key;
    FlagValidity validity;

    public FlagStatusCondition(JObject json)
    {
        object value = json["value"].ToObject<object>();
        if (!(value is bool || value is int || value is float || value is string))
        {
            throw new System.ArgumentException("Value must be bool, int, float, or string");
        }

        this.value = value;
        this.key = json["key"].ToString();
        this.validity = EnumHelper.ParseEnum<FlagValidity>(json["validity"]?.ToString()) ?? FlagValidity.Game;
    }

    public override bool IsFulfilled()
    {
        return object.Equals(Game.Instance.GetFlag(this.validity, this.key), this.value);
    }
}