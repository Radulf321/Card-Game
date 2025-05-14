using System;
using UnityEngine;

#nullable enable

public class DialogOptionHandler : MonoBehaviour
{
    private Action? onClickAction;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnClick() {
        this.onClickAction?.Invoke();
    }

    public void SetAction(Action action) {
        this.onClickAction = action;
    }

    public void SetText(string text) {
        transform.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = text;
    }
}
