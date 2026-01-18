using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using UnityEngine.Localization.Settings;

#nullable enable

public class PlayOwnerEffect : GameEffect
{
    private Card owner;


    public PlayOwnerEffect(Card? owner)
    {
        if (owner == null)
        {
            throw new ArgumentException("Owner must not be null for PlayOwnerEffect");
        }
        this.owner = owner;
    }

    public PlayOwnerEffect(JObject json, Card? owner) : this(
        owner: owner
    )
    {
    }

    public override void applyEffect(Enemy? target = null)
    {
        owner.Play(true);
    }

    public override GameEffect Clone(Card? newOwner)
    {
        return new PlayOwnerEffect(newOwner ?? this.owner);
    }

    public override async Task<string> getDescription()
    {
        return await AsyncHelper.HandleToTask(LocalizationSettings.StringDatabase.GetLocalizedStringAsync("CardStrings", "PlayOwner"));
    }

    public override async Task<string> getTriggerDescription()
    {
        return await AsyncHelper.HandleToTask(LocalizationSettings.StringDatabase.GetLocalizedStringAsync("CardStrings", "TriggerPlayOwner"));
    }
}