using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

#nullable enable

public class EnemyHandler : MonoBehaviour, IViewUpdater
{
    private Enemy? enemy;
    private string? imagePath;
    private float? previousHeight;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Game.Instance.SubscribeToTriggerMessages(OnTriggerMessage);
    }

    public void SetEnemy(Enemy enemy)
    {
        this.enemy = enemy;
        this.updateView();
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

    public async void updateView()
    {
        string? imagePath = this.enemy?.GetImagePath();
        Image image = transform.Find("EnemyImage").GetComponent<Image>();
        if (imagePath != this.imagePath)
        {
            if (imagePath != null)
            {
                Sprite sprite = Resources.Load<Sprite>(imagePath);
                image.sprite = sprite;

                transform.GetComponent<AspectRatioFitter>().aspectRatio = sprite.rect.width / sprite.rect.height;
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
        transform.Find("RequirementsInfo").GetComponent<TMPro.TextMeshProUGUI>().text = string.Join("\n", requirementsTexts);

        if ((previousHeight != GetComponent<RectTransform>().rect.height) && (imagePath != null))
        {
            previousHeight = GetComponent<RectTransform>().rect.height;
            float textHeight = transform.Find("RequirementsInfo").GetComponent<TMPro.TextMeshProUGUI>().preferredHeight;
            float imageHeight = previousHeight.Value - textHeight;
            // Must not be null as imagePath is not null -> A sprite is set
            Sprite sprite = image.sprite!;
            float imageAspectRatio = sprite.rect.width / sprite.rect.height;
            float imageWidth = imageHeight * imageAspectRatio;
            transform.GetComponent<AspectRatioFitter>().aspectRatio = imageWidth / previousHeight.Value;
        }
    }

    public float GetAspectRatio()
    {
        return transform.GetComponent<AspectRatioFitter>().aspectRatio;
    }

    protected virtual void OnRectTransformDimensionsChange()
    {
        this.updateView();
    }
}
