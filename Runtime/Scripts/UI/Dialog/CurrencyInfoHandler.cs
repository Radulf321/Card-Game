using System;
using System.Collections.Generic;
using UnityEngine;

#nullable enable

public class CurrencyInfoHandler : MonoBehaviour, IViewUpdater
{
    [SerializeField] private CurrencyContext context = CurrencyContext.Dialog;

    public void Start()
    {
        updateView();
        Game.Instance.SubscribeToTriggerMessages(OnTriggerMessage);
    }

    public void OnDestroy()
    {
        Game.Instance.UnsubscribeFromTriggerMessages(OnTriggerMessage);
    }

    public async void updateView()
    {
        List<String> currencyStrings = new List<String>();
        foreach (Requirement requirement in Game.Instance.GetPlayer().GetGoalManager().GetRequirements())
        {
            currencyStrings.Add(await requirement.GetDescription());
        }
        foreach (String currencyID in Game.Instance.GetCurrencies())
        {
            CurrencyData? currencyData = Game.Instance.GetCurrencyData(currencyID);
            if (currencyData == null || !currencyData.HasContext(context))
            {
                continue;
            }
            int currencyAmount = Game.Instance.GetPlayer().GetCurrency(currencyID);
            string format = currencyData.GetFormat();
            currencyStrings.Add(JSONHelper.ReplaceVariablesInString(format, new Dictionary<string, object>()
            {
                {"amount", currencyAmount.ToString()}
            }).ToString());
        }
        transform.GetComponent<TMPro.TextMeshProUGUI>().text = string.Join("\n", currencyStrings);
    }

    private void OnTriggerMessage(TriggerMessage message)
    {
        // We could restrict further by checking that it's a relevant goal/currency
        // But for now, just update on any change to goals/currencies since it's not too expensive
        switch (message.GetTriggerType())
        {
            case TriggerType.AddCurrency:
            case TriggerType.AddProtection:
            case TriggerType.AddGoal:
                updateView();
                break;
        }
    }
}
