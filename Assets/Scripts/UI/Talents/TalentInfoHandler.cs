using System;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using UnityEngine.Localization.Settings;
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
        Transform rewardsArea = descriptionArea.Find("RewardsArea");
        rightArea.Find("DescriptionScroll").GetComponentInChildren<Scrollbar>().value = 1;

        for (int i = 0; i < rewardsArea.childCount; i++)
        {
            Destroy(rewardsArea.GetChild(i).gameObject);
        }

        foreach (Reward reward in talent.GetRewards())
        {
            if (reward is CardReward cardReward)
            {
                GameObject cardObject = Instantiate(cardPrefab!);
                cardObject.GetComponentInChildren<CardHandler>().SetCard(cardReward.GetCard());
                cardObject.GetComponentInChildren<CardHandler>().SetActive(false);
                float cardHeight = Mathf.Max(240, 0.38f * myHeight);
                cardObject.GetComponent<RectTransform>().sizeDelta = new UnityEngine.Vector2(cardHeight * cardObject.GetComponent<AspectRatioFitter>().aspectRatio, cardHeight);
                cardObject.transform.SetParent(rewardsArea, false);
            }
        }

        Transform confirmButton = rightArea.Find("ButtonArea").Find("ConfirmButton");
        confirmButton.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = talent.IsPurchased() ?
                LocalizationSettings.StringDatabase.GetLocalizedString("UIStrings", "TalentAlreadyPurchased") :
                LocalizationSettings.StringDatabase.GetLocalizedString("UIStrings", "TalentPurchase", arguments: new Dictionary<string, object> { { "cost", GetCostString(talent.GetCost()) } });
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
            DialogHandler.dialogFinish = () => {
                Game.Instance.EndRound();
            };
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
