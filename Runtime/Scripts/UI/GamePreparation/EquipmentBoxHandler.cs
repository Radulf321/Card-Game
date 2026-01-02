using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

#nullable enable
public class EquipmentBoxHandler : MonoBehaviour, IPointerDownHandler
{
    private Equipment? equipment;
    bool selected = false;
    bool needUpdate = false;
    Action<Equipment?>? onClickAction;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (needUpdate)
        {
            transform.GetComponentInParent<LayoutElement>().preferredWidth = transform.GetComponentInParent<RectTransform>().rect.height * transform.GetComponentInParent<AspectRatioFitter>().aspectRatio;
            transform.Find("EquipmentIcon").GetComponent<Image>().sprite = (equipment != null) ? Resources.Load<Sprite>(equipment.GetIconPath()) : null;
            transform.GetComponent<Image>().color = selected ? Game.Instance.GetEquipmentManager().GetSelectedColor() : Color.white;
            needUpdate = false;
        }
    }

    public void SetEquipment(Equipment? equipment)
    {
        this.equipment = equipment;
        needUpdate = true;
    }

    public void SetSelected(bool selected)
    {
        this.selected = selected;
        needUpdate = true;
    }

    public void SetOnClickAction(Action<Equipment?>? onClickAction)
    {
        this.onClickAction = onClickAction;
    }

    protected virtual void OnRectTransformDimensionsChange()
    {
        needUpdate = true;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        this.onClickAction?.Invoke(this.equipment);
    }
}
