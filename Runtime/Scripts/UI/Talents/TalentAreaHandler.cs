using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class TalentAreaHandler : MonoBehaviour
{
    public GameObject talentPrefab;
    private Transform parent;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        this.parent = GameObject.Find("Canvas").transform;
        parent.Find("Image").GetComponent<Image>().sprite = Resources.Load<Sprite>(Game.Instance.GetCurrentCombatTarget().GetTalentBackgroundPath());
        renderTalents();
        Game.Instance.SendTriggerMessage(new TriggerMessage(TriggerType.TalentTree));
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void SkipPurchase()
    {
        Game.Instance.EndRound();
    }

    private void renderTalents()
    {
        List<Talent> talents = Game.Instance.GetCurrentCombatTarget().GetTalents();
        Dictionary<string, GameObject> talentObjects = new Dictionary<string, GameObject>();
        Rect parentSize = parent.GetComponent<RectTransform>().rect;
        float talentHeight = Mathf.Max(240f, parentSize.height * 0.4f);
        float talentWidth = talentHeight * CardHandler.standardWidth / CardHandler.standardHeight;
        float heightOffset = 2f / 3f * talentHeight;
        float maxY = 0;

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

            float currentY = (talentsToAdd.Count - 1) * heightOffset;
            maxY = Mathf.Max(maxY, currentY);

            foreach (Talent talent in talentsToAdd)
            {
                GameObject cardObject = Instantiate(talentPrefab);
                cardObject.transform.SetParent(this.transform, false);
                CardHandler cardHandler = cardObject.GetComponentInChildren<CardHandler>();
                cardHandler.SetTalent(talent);
                cardHandler.SetHeight(talentHeight);
                RectTransform cardRect = cardObject.GetComponent<RectTransform>();
                cardRect.anchorMin = new Vector2(0, 0.5f);
                cardRect.anchorMax = new Vector2(0, 0.5f);
                cardRect.pivot = new Vector2(0, 0.5f);
                cardRect.anchoredPosition = new Vector2(currentX, currentY);
                talentObjects.Add(talent.GetID(), cardObject);
                foreach (string prerequisite in talent.GetPrerequisites())
                {
                    DrawUILine(talentObjects[prerequisite].GetComponent<RectTransform>(), cardRect);
                }
                currentY -= 2 * heightOffset;
            }
            currentX += 3 * talentWidth;
        }

        parent.Find("AvailableExperienceInfo").GetComponent<TMPro.TextMeshProUGUI>().text = string.Join("\n", Game.Instance.GetCurrentCombatTarget().GetExperience().Select((KeyValuePair<string, int> kvp) => Game.Instance.GetExperienceTypeInlineIcon(kvp.Key) + ": " + kvp.Value.ToString()));
        GetComponent<RectTransform>().sizeDelta = new Vector2(Mathf.Max(parentSize.width, currentX - talentWidth), Mathf.Max(parentSize.height, 2 * (maxY + talentHeight)));
    }

    private void DrawUILine(RectTransform startRect, RectTransform endRect)
    {
        Vector2 start = startRect.anchoredPosition + startRect.rect.center;
        Vector2 end = endRect.anchoredPosition + endRect.rect.center;
        GameObject line = new GameObject("UILine");
        line.transform.SetParent(transform, false);
        line.transform.SetSiblingIndex(0); // Set the line to be behind other UI elements

        RectTransform rectTransform = line.AddComponent<RectTransform>();
        Image image = line.AddComponent<Image>();
        image.color = Color.white; // Set the line color

        // Set the size and position
        Vector2 direction = end - start;
        // Assure same anchoring as talents
        rectTransform.anchorMin = startRect.anchorMin;
        rectTransform.anchorMax = startRect.anchorMax;
        rectTransform.sizeDelta = new Vector2(direction.magnitude, 2f); // Line width is 2
        rectTransform.pivot = new Vector2(0, 0.5f); // Pivot at the start of the line
        rectTransform.anchoredPosition = start;

        // Rotate the line to match the direction
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        rectTransform.rotation = Quaternion.Euler(0, 0, angle);
    }
}
