#nullable enable

using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;

public enum GoalUsageType
{
    Reversed,
    Negative,
    Default,
    Protection
}

public enum GoalDisplayType
{
    Regular,
    Reversed
}

public class GoalData : NamedIconData {
    private Dictionary<GoalUsageType, string> icons;
    private Dictionary<GoalUsageType, string> names;
    private GoalDisplayType displayType;

    public GoalData(JObject goalData) : base(goalData)
    {
        Dictionary<GoalUsageType, string> icons = new Dictionary<GoalUsageType, string>();
        if (this.iconName != null)
        {
            icons[GoalUsageType.Default] = this.iconName;
        }
        if (goalData["icons"] != null)
        {
            foreach (JProperty iconProperty in goalData["icons"]!)
            {
                GoalUsageType? iconType = EnumHelper.ParseEnum<GoalUsageType>(iconProperty.Name);
                if (iconType != null)
                {
                    icons[iconType.Value] = iconProperty.Value.ToString()!;
                }
            }
        }
        this.icons = icons;
        Dictionary<GoalUsageType, string> names = new Dictionary<GoalUsageType, string>();
        if (this.name != null)
        {
            names[GoalUsageType.Default] = this.name;
        }
        if (goalData["names"] != null)
        {
            foreach (JProperty nameProperty in goalData["names"]!)
            {
                GoalUsageType? nameType = EnumHelper.ParseEnum<GoalUsageType>(nameProperty.Name);
                if (nameType != null)
                {
                    names[nameType.Value] = LocalizationHelper.GetLocalizedString(nameProperty.Value as JObject)!;
                }
            }
        }
        this.names = names;
        this.displayType = EnumHelper.ParseEnum<GoalDisplayType>(goalData["displayType"]?.ToString()) ?? GoalDisplayType.Regular;
    }

    public override string GetName() {
        string? result = GetName(GoalUsageType.Default);
        if (result == null) {
            throw new Exception("No default name found for goal");
        }
        return result;
    }

    public string? GetName(GoalUsageType type) {
        return names[type];
    }

    public override string GetInlineIcon() {
        return GetInlineIcon(GoalUsageType.Default);
    }

    public string GetInlineIcon(GoalUsageType type) {
        if (!icons.ContainsKey(type)) {
            return this.GetName();
        }
        return "<sprite name=\"" + icons[type] + "\">";
    }

    public override Sprite? GetIcon() {
        return GetIcon(GoalUsageType.Default);
    }

    public Sprite? GetIcon(GoalUsageType type) {
        if (!icons.ContainsKey(type)) {
            return null;
        }
        return Game.Instance?.GetIcon(icons[type]);
    }

    public GoalDisplayType GetDisplayType() {
        return displayType;
    }
}