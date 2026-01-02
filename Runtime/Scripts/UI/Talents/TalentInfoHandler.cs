using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

#nullable enable

public class TalentInfoHandler : MonoBehaviour
{
    public GameObject? cardPrefab;

    private Talent? talent;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ShowTalent(Talent talent)
    {
        CombatTarget currentTarget = Game.Instance.GetCurrentCombatTarget();
        foreach (string prerequisiteID in talent.GetPrerequisites())
        {
            Talent? prerequisite = currentTarget.GetTalent(prerequisiteID);
            if (prerequisite == null)
            {
                throw new Exception("Prerequisite Talent does not exist: " + prerequisiteID);
            }
            
            if (!prerequisite.IsPurchased())
            {
                Debug.Log("Prerequisites not met");
                return;
            }
        }
        this.talent = talent;
        transform.gameObject.SetActive(true);
        transform.Find("ImageArea").GetComponentInChildren<Image>().sprite = Resources.Load<Sprite>(talent.GetImagePath());
        float myHeight = transform.GetComponent<RectTransform>().rect.height;
        Transform rightArea = transform.Find("RightArea");
        rightArea.GetComponent<VerticalLayoutGroup>().spacing = myHeight * 0.02f;
        Transform descriptionArea = rightArea.Find("DescriptionScroll").Find("Mask").Find("DescriptionArea");
        float titleSize = myHeight * 0.06f;
        float descriptionSize = myHeight * 0.04f;
        descriptionArea.GetComponent<VerticalLayoutGroup>().spacing = titleSize;
        TMPro.TextMeshProUGUI titleText = descriptionArea.Find("Title").GetComponent<TMPro.TextMeshProUGUI>();
        titleText.text = talent.GetTitle();
        titleText.fontSize = titleSize;
        TMPro.TextMeshProUGUI descriptionText = descriptionArea.Find("Description").GetComponent<TMPro.TextMeshProUGUI>();
        descriptionText.text = talent.GetInfoDescription();
        descriptionText.fontSize = descriptionSize;
        descriptionArea.Find("RewardsTitle").GetComponent<TMPro.TextMeshProUGUI>().fontSize = titleSize;
        rightArea.Find("DescriptionScroll").GetComponentInChildren<Scrollbar>(includeInactive: true).value = 1;

        Transform rewardDisplayArea = descriptionArea.Find("RewardDisplayArea");
        rewardDisplayArea.GetComponent<LayoutElement>().preferredHeight = Mathf.Max(240, 0.38f * myHeight);
        rewardDisplayArea.GetComponent<LayoutElement>().flexibleHeight = 0;
        rewardDisplayArea.GetComponent<RewardAreaHandler>().SetRewards(talent.GetRewards());

        Transform confirmButton = rightArea.Find("ButtonArea").Find("ConfirmButton");
        AsyncOperationHandle<string> confirmButtonLocalization = talent.IsPurchased() ?
                LocalizationSettings.StringDatabase.GetLocalizedStringAsync("UIStrings", "TalentAlreadyPurchased") :
                LocalizationSettings.StringDatabase.GetLocalizedStringAsync("UIStrings", "TalentPurchase", arguments: new Dictionary<string, object> { { "cost", GetCostString(talent.GetCost()) } });
        confirmButtonLocalization.Completed += op => {
            confirmButton.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = op.Result;
        };
        confirmButton.GetComponent<Button>().interactable = !talent.IsPurchased();
    }

    public void Close()
    {
        transform.gameObject.SetActive(false);
    }

    public void PurchaseTalent()
    {
        try
        {
            talent?.Purchase();
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
            return;
        }
    }

    private string GetCostString(Dictionary<string, int> cost)
    {
        string costString = "";
        foreach (KeyValuePair<string, int> costEntry in cost)
        {
            costString += "-" + costEntry.Value + Game.Instance.GetExperienceTypeInlineIcon(costEntry.Key);
        }
        return costString;
    }
}
