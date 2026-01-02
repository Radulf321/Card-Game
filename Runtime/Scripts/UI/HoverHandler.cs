using UnityEngine;
using UnityEngine.EventSystems;

#nullable enable
public class HoverHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    bool active = true;
    bool dragging = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Game.Instance.SubscribeToTriggerMessages(OnTriggerMessage);
    }

    // Update is called once per frame
    void Update()
    {

    }

    protected virtual void OnDestroy()
    {
        Game.Instance.UnsubscribeFromTriggerMessages(OnTriggerMessage);
    }

    public void SetActive(bool active)
    {
        this.active = active;
    }

    // Hover effect: Bring the card to the top
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!active || dragging)
        {
            return;
        }

        StartHover();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!active || dragging)
        {
            return;
        }

        StopHover();
    }

    public void StopHover()
    {
        Canvas canvas = GetComponent<Canvas>();
        canvas.overrideSorting = false;
        canvas.sortingOrder = 0;
        transform.GetComponent<CardHandler>()?.HideTooltip();
    }

    public void StartHover()
    {
        Canvas canvas = GetComponent<Canvas>();
        canvas.overrideSorting = true;
        canvas.sortingOrder = 100; // Bring to front
        transform.GetComponent<CardHandler>()?.ShowTooltip();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        StopHover();
    }

    private void OnTriggerMessage(TriggerMessage message)
    {
        switch (message.GetTriggerType())
        {
            case TriggerType.CardDragEnd:
                this.dragging = false;
                break;

            case TriggerType.CardDragStart:
                this.dragging = true;
                break;

            default:
                break;
        }
    }
}
