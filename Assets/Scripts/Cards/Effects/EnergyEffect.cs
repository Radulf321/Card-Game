using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine.Localization.Settings;

public class EnergyEffect : CardEffect {
    private int amount;
    private bool maxEnergy;

    public EnergyEffect(int amount, bool maxEnergy = false) {
        this.amount = amount;
        this.maxEnergy = maxEnergy;
    }

    public EnergyEffect(JObject json) {
        this.maxEnergy = json["maxEnergy"]?.ToObject<bool>() ?? false;
        this.amount = json["amount"]?.ToObject<int>() ?? 0;
    }

    public override void applyEffect() {
        // Assuming RoundHandler has a method to apply the effect
        CombatHandler.instance.gainEnergy(this.amount, this.maxEnergy);
    }

    public override string getDescription() {
        return LocalizationSettings.StringDatabase.GetLocalizedString("CardStrings", "Energy",
            arguments: new Dictionary<string, object> {
                { "amount", this.amount },
                { "maxEnergy", this.maxEnergy }
            }
        );
    }

    public override CardEffect Clone(Card newOwner)
    {
        return new EnergyEffect(this.amount, this.maxEnergy);
    }
}