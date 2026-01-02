using UnityEngine;
using TMPro;

#nullable enable
public class TooltipHandler : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
    }

    public async void ShowTooltip(string description)
    {
        TextMeshProUGUI tooltipText =
             transform.Find("TooltipText").GetComponent<TextMeshProUGUI>();
        tooltipText.text = description;
        RectTransform tooltipRect = transform.GetComponent<RectTransform>();
        tooltipRect.sizeDelta = new Vector2(tooltipRect.sizeDelta.x, tooltipText.preferredHeight + 100);
        transform.gameObject.SetActive(true);
    }

    public void HideTooltip()
    {
        transform.gameObject.SetActive(false);
    }
}
