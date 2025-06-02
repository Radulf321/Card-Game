using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Linq;
using System;

#nullable enable
public class HoverHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    bool active = true;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetActive(bool active)
    {
        this.active = active;
    }

    // Hover effect: Bring the card to the top
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!active)
        {
            return;
        }
        Canvas canvas = GetComponent<Canvas>();
        canvas.overrideSorting = true; // Enable sorting order override
        canvas.sortingOrder = 100; // Bring the card to the top
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!active)
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
    }
}
