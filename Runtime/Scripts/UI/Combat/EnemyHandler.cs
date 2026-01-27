using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

#nullable enable

public class EnemyHandler : MonoBehaviour, IViewUpdater
{
    private Enemy? enemy;
    private string? imagePath;
    private float? height;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Game.Instance.SubscribeToTriggerMessages(OnTriggerMessage);
    }

    public void SetEnemy(Enemy enemy)
    {
        this.enemy = enemy;
        //transform.Find("EnemyName").GetComponent<Text>().text = enemy.GetName();
        //transform.Find("EnemyImage").GetComponent<Image>().sprite = Resources.Load<Sprite>(enemy.GetImagePath());
        //transform.Find("EnemyHealth").GetComponent<Text>().text = $"{enemy.GetCurrentHealth()}/{enemy.GetMaxHealth()}";
    }

    public Enemy? GetEnemy()
    {
        return this.enemy;
    }

    // Update is called once per frame
    void Update()
    {

    }

    protected virtual void OnDestroy()
    {
        Game.Instance.UnsubscribeFromTriggerMessages(OnTriggerMessage);
    }

    private void OnTriggerMessage(TriggerMessage message)
    {
        switch (message.GetTriggerType())
        {
            case TriggerType.CardDragEnd:
            case TriggerType.CardDragStart:
                break;
            default:
                break;
        }
    }

    public void updateView()
    {
        _ = this.updateViewAsync();
    }

    public async Task updateViewAsync()
    {
        GameEffect? plannedEffect = this.enemy?.GetPlannedEffect();
        string effectText = (plannedEffect != null) ? await plannedEffect.GetIconDescription() : "-";
        transform.Find("PlannedEffectInfo").Find("PlannedEffectText").GetComponent<TMPro.TextMeshProUGUI>().text = effectText;

        string? imagePath = this.enemy?.GetImagePath();
        Image image = transform.Find("EnemyImage").GetComponent<Image>();
        if (imagePath != this.imagePath)
        {
            if (imagePath != null)
            {
                Sprite sprite = Resources.Load<Sprite>(imagePath);
                image.sprite = sprite;
            }
            else
            {
                image.sprite = null;
            }
            this.imagePath = imagePath;
        }

        List<string> requirementsTexts = new List<string>();
        if (this.enemy != null)
        {
            foreach (Requirement requirement in this.enemy.GetRequirements())
            {
                requirementsTexts.Add(await requirement.GetDescription());
            }
        }
        TMPro.TextMeshProUGUI requirementsInfo = transform.Find("RequirementsInfo").GetComponent<TMPro.TextMeshProUGUI>();
        requirementsInfo.text = string.Join("\n", requirementsTexts);
        if (this.height != null) {
            float textHeight = transform.Find("RequirementsInfo").GetComponent<TMPro.TextMeshProUGUI>().preferredHeight + 
                transform.Find("PlannedEffectInfo").GetComponent<LayoutElement>().preferredHeight;
            float imageHeight = height.Value - textHeight;
            // Must not be null as imagePath is not null -> A sprite is set
            Sprite? sprite = image.sprite;
            float? imageWidth;
            if (sprite != null)
            {
                float imageAspectRatio = sprite.rect.width / sprite.rect.height;
                imageWidth = imageHeight * imageAspectRatio;
            }
            else
            {
                imageWidth = null;
            }

            float textWidth = requirementsInfo.preferredWidth;

            UnityEngine.Debug.Log("Setting EnemyHandler size: " + height.Value + "x" + (imageWidth ?? 0) + " and text width " + textWidth);

            transform.GetComponent<RectTransform>().sizeDelta = new Vector2(Mathf.Max(imageWidth ?? 0, textWidth), height.Value);
        }
    }

    public async Task SetHeight(float height)
    {
        this.height = height;
        await this.updateViewAsync();
    }

    public float GetWidth()
    {
        return transform.GetComponent<RectTransform>().sizeDelta.x;
    }

    protected virtual void OnRectTransformDimensionsChange()
    {
        this.updateView();
    }
}
