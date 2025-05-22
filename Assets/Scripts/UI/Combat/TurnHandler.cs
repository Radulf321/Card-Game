using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TurnHandler : MonoBehaviour, IViewUpdater
{
    public Turn turn;
    public Sprite firstTurnStartSprite;
    public Sprite regularTurnStartSprite;
    public Sprite regularTurnEndSprite;
    public Sprite lastTurnEndSprite;

    private bool updatePreferredWidth = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (updatePreferredWidth) {
            Transform centerTransform = transform.Find("Center");
            centerTransform.GetComponent<LayoutElement>().preferredWidth = centerTransform.GetComponentInChildren<TextMeshProUGUI>().preferredWidth;
            updatePreferredWidth = false;
        }
    }

    void OnRectTransformDimensionsChange()
    {
        updateView();
    }

    public void SetTurn(Turn turn) {
        this.turn = turn;
        List<Turn> allTurns = CombatHandler.instance.getTurns();
        Image startImage = transform.GetChild(0).GetComponent<Image>();
        startImage.sprite = (turn == allTurns.First()) ? firstTurnStartSprite : regularTurnStartSprite;

        Image endImage = transform.GetChild(2).GetComponent<Image>();
        endImage.sprite = (turn == allTurns.Last()) ? lastTurnEndSprite : regularTurnEndSprite;
        updateView();
    }

    public Turn GetTurn() {
        return this.turn;
    }

    public void updateView() {
        Transform centerTransform = transform.Find("Center");
        List<Turn> allTurns = CombatHandler.instance.getTurns();
        int currentTurn = CombatHandler.instance.getCurrentTurnIndex();
        int turnIndex = allTurns.IndexOf(turn);
        Color color;

        if (turnIndex < currentTurn) {
            color = Theme.inactiveColor;
        }
        else {
            color = Theme.roundFulfilledColor;
            for (int i = currentTurn; i <= turnIndex; i++) {
                if (!allTurns[i].areRequirementsFulfilled()) {
                    color = Theme.roundNotFulfilledColor;
                    break;
                }
            }
        }

        if (turnIndex != currentTurn) {
            color.a = 0.4f;
        }

        float height = transform.GetComponent<RectTransform>().rect.height;
        HorizontalLayoutGroup layoutGroup = GetComponent<HorizontalLayoutGroup>();
        int padding;
        if (turnIndex == currentTurn) {
            padding = 0;
        }
        else {
            padding = Mathf.FloorToInt(height * 0.1f);
        }
        layoutGroup.padding.bottom = padding;
        layoutGroup.padding.top = padding;
        height -= 2 * padding;
        TextMeshProUGUI text =  centerTransform.GetComponentInChildren<TextMeshProUGUI>();
        text.fontSizeMax = height / 3;
        List<string> texts = turn.getRequirements().Select(req => req.toString()).ToList();
        texts.AddRange(turn.getEffects().Select(effect => effect.getTurnEffectDescription()));
        text.text = string.Join("\n", texts);
        foreach (Image child in GetComponentsInChildren<Image>()) {
            child.color = color;
        }
        foreach (string name in new string[] { "Begin", "End" }) {
            transform.Find(name).GetComponent<LayoutElement>().preferredWidth = height / 2;
        }
        // Delay updating preferred width until next update so the text updates its preferred width
        updatePreferredWidth = true;
    }
}
