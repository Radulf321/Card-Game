#nullable enable

using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.Localization.Settings;

public class BuffFactory
{
    private int duration;
    private Modifier modifier;

    public BuffFactory(Modifier modifier, int duration)
    {
        this.modifier = modifier;
        this.duration = duration;
    }

    public BuffFactory(JObject json) : this(
        modifier: new Modifier(json["modifier"] as JObject),
        duration: json["duration"]!.ToObject<int>()
    )
    {

    }

    public Buff CreateBuff()
    {
        return new Buff(this.modifier.Clone(), this.duration);
    }

    public async Task<string> GetCaption()
    {
        string modifierText = await modifier.GetCaption();

        return await AsyncHelper.HandleToTask(LocalizationSettings.StringDatabase.GetLocalizedStringAsync("RewardStrings", "BuffCaption",
            arguments: new Dictionary<string, object> {
                { "modifier", modifierText },
                { "duration", this.duration }
            }
        ));
    }

    public Sprite GetSprite()
    {
        return this.modifier.GetSprite();
    }
}