using System;
using UnityEngine;
using UnityEngine.UI;

#nullable enable

public class EnemyHandler : MonoBehaviour, IViewUpdater
{
    private Enemy? enemy;
    private string? imagePath;

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

    public void updateView()
    {
        string? imagePath = this.enemy?.GetImagePath();
        if (imagePath != this.imagePath)
        {
            if (imagePath != null)
            {
                transform.GetComponent<Image>().sprite = Resources.Load<Sprite>(imagePath);
            }
            else
            {
                transform.GetComponent<Image>().sprite = null;
            }
            this.imagePath = imagePath;
        }
        // TODO: Update enemy view
    }
}
