using System.Linq;
using Newtonsoft.Json.Linq;

public abstract class AmountCalculation {
    public static AmountCalculation FromJson(JToken json) {
        switch (json.Type) {
            case JTokenType.Integer:
                return new ConstantAmountCalculation(json.ToObject<int>());

            case JTokenType.Object:
                JObject jsonObject = (JObject)json;
                string type = jsonObject["type"]?.ToString() ?? "";
                switch (type) {
                    case "linear":
                        return new LinearAmountCalculation(jsonObject);
                    case "constant":
                        return new ConstantAmountCalculation(jsonObject);
                    default:
                        throw new System.Exception("Invalid amount calculation type: " + type);
                }

            default:
                throw new System.Exception("Invalid amount calculation type: " + json.Type);
        }
    }

    abstract public int GetValue(Card card);
}