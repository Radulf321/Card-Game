using System;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using UnityEngine.Localization.Settings;

#nullable enable
public class AfterPlayEffect : GameEffect {
    private CardAfterPlay afterPlay;
    private Card owner;

    public AfterPlayEffect(CardAfterPlay afterPlay, Card owner)
    {
        this.afterPlay = afterPlay;
        this.owner = owner;
    }

    public AfterPlayEffect(JObject json, Card owner)
    {
        this.afterPlay = CardAfterPlayHelper.FromJson(json["afterPlay"]!);
        this.owner = owner;
    }

    public override void applyEffect() {
        this.owner.SetAfterPlay(this.afterPlay);
    }

    public override GameEffect Clone(Card? newOwner)
    {
        return new AfterPlayEffect(this.afterPlay, newOwner ?? this.owner);
    }

    public override Task<string> getDescription() {
        switch (this.afterPlay)
        {
            case CardAfterPlay.RemovePermanent:
                return AsyncHelper.HandleToTask(LocalizationSettings.StringDatabase.GetLocalizedStringAsync("CardStrings", "RemovePermanent"));

            default:
                throw new NotImplementedException();
        }
    }
}