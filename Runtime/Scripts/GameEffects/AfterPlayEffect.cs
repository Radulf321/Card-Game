using System;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using UnityEngine.Localization.Settings;

#nullable enable
public class AfterPlayEffect : GameEffect {
    private CardAfterPlay afterPlay;
    private Card owner;

    public AfterPlayEffect(CardAfterPlay afterPlay, IEffectOwner owner)
    {
        if (owner is Card cardOwner)
        {
            this.owner = cardOwner;
        }
        else
        {
            throw new ArgumentException("Owner of AfterPlayEffect must be a Card");
        }
        this.afterPlay = afterPlay;
    }

    public AfterPlayEffect(JObject json, IEffectOwner owner)
    {
        this.afterPlay = CardAfterPlayHelper.FromJson(json["afterPlay"]!);
        if (owner is Card cardOwner)
        {
            this.owner = cardOwner;
        }
        else
        {
            throw new ArgumentException("Owner of AfterPlayEffect must be a Card");
        }
    }

    public override void applyEffect(Enemy? target = null) {
        this.owner.SetAfterPlay(this.afterPlay);
    }

    public override GameEffect Clone(IEffectOwner? newOwner)
    {
        return new AfterPlayEffect(this.afterPlay, newOwner ?? this.owner);
    }

    public override Task<string> getDescription() {
        switch (this.afterPlay)
        {
            case CardAfterPlay.RemovePermanent:
                return AsyncHelper.HandleToTask(LocalizationSettings.StringDatabase.GetLocalizedStringAsync("CardStrings", "RemovePermanent"));

            case CardAfterPlay.RemoveCurrentGame:
                return AsyncHelper.HandleToTask(LocalizationSettings.StringDatabase.GetLocalizedStringAsync("CardStrings", "RemoveCurrent"));

            default:
                throw new NotImplementedException();
        }
    }

    public override Task<string?> GetInternalIconDescription() {
        switch (this.afterPlay)
        {
            case CardAfterPlay.RemovePermanent:
                return AsyncHelper.HandleToTask(LocalizationSettings.StringDatabase.GetLocalizedStringAsync("CardStrings", "RemovePermanentIcon"));

            case CardAfterPlay.RemoveCurrentGame:
                return AsyncHelper.HandleToTask(LocalizationSettings.StringDatabase.GetLocalizedStringAsync("CardStrings", "RemoveCurrentIcon"));

            default:
                return Task.FromResult<string?>(null);
        }
    }
}