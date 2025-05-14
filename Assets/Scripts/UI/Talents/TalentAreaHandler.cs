using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TalentAreaHandler : MonoBehaviour
{
    public GameObject talentPrefab;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        transform.parent.Find("Image").GetComponent<Image>().sprite = Resources.Load<Sprite>(Game.Instance.GetCurrentCombatTarget().GetTalentBackgroundPath());
        renderTalents();
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void SkipPurchase()
    {
        Game.Instance.EndTurn();
    }

    private void renderTalents()
    {
        List<Talent> talents = Game.Instance.GetCurrentCombatTarget().GetTalents();
        Dictionary<string, GameObject> talentObjects = new Dictionary<string, GameObject>();
        Rect mySize = GetComponent<RectTransform>().rect;
        float talentHeight = Mathf.Max(240f, mySize.height * 0.4f);
        float talentWidth = talentHeight * talentPrefab.GetComponent<AspectRatioFitter>().aspectRatio;

        float currentX = talentWidth;

        for (int i = 0; i < transform.childCount; i++)
        {
            Destroy(transform.GetChild(i).gameObject);
        }

        while (talentObjects.Count < talents.Count)
        {
            List<Talent> talentsToAdd = new List<Talent>();
            foreach (Talent talent in talents)
            {
                if (!talentObjects.ContainsKey(talent.GetID()))
                {
                    bool canAdd = true;
                    foreach (string prerequisite in talent.GetPrerequisites())
                    {
                        if (!talentObjects.ContainsKey(prerequisite))
                        {
                            canAdd = false;
                            break;
                        }
                    }
                    if (canAdd)
                    {
                        talentsToAdd.Add(talent);
                    }
                }
            }
            if (talentsToAdd.Count == 0)
            {
                throw new System.Exception("Cannot add talents, no prerequisites met.");
            }

            foreach (Talent talent in talentsToAdd)
            {
                GameObject cardObject = Instantiate(talentPrefab);
                cardObject.GetComponentInChildren<CardHandler>().SetTalent(talent);
                cardObject.transform.SetParent(this.transform, false);
                RectTransform cardRect = cardObject.GetComponent<RectTransform>();
                cardRect.anchorMin = new Vector2(0, 0.5f);
                cardRect.anchorMax = new Vector2(0, 0.5f);
                cardRect.pivot = new Vector2(0, 0.5f);
                cardRect.anchoredPosition = new Vector2(currentX, 0);
                cardRect.sizeDelta = new Vector2(talentWidth, talentHeight);
                talentObjects.Add(talent.GetID(), cardObject);
                foreach (string prerequisite in talent.GetPrerequisites())
                {
                    DrawUILine(cardRect, talentObjects[prerequisite].GetComponent<RectTransform>());
                }
            }
            currentX += 3 * talentWidth;
        }

        transform.parent.Find("AvailableExperienceInfo").GetComponent<TMPro.TextMeshProUGUI>().text = string.Join("\n", Game.Instance.GetCurrentCombatTarget().GetExperience().Select((KeyValuePair<string, int> kvp) => Game.Instance.GetExperienceTypeInlineIcon(kvp.Key) + ": " + kvp.Value.ToString()));
    }

    private void DrawUILine(RectTransform startRect, RectTransform endRect)
    {
        Vector2 start = startRect.TransformPoint(startRect.rect.center);
        Vector2 end = endRect.TransformPoint(endRect.rect.center);
        GameObject line = new GameObject("UILine");
        line.transform.SetParent(transform, false);
        line.transform.SetSiblingIndex(0); // Set the line to be behind other UI elements

        RectTransform rectTransform = line.AddComponent<RectTransform>();
        Image image = line.AddComponent<Image>();
        image.color = Color.white; // Set the line color

        // Set the size and position
        Vector2 direction = end - start;
        rectTransform.sizeDelta = new Vector2(direction.magnitude, 2f); // Line width is 2
        rectTransform.pivot = new Vector2(0, 0.5f); // Pivot at the start of the line
        rectTransform.position = start;

        // Rotate the line to match the direction
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        rectTransform.rotation = Quaternion.Euler(0, 0, angle);
    }
}
