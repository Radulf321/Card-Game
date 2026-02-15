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

    public void updateView()
    {
        List<String> currencyStrings = new List<String>();
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
        if (message.GetTriggerType() == TriggerType.AddCurrency)
        {
            updateView();
        }
    }
}
