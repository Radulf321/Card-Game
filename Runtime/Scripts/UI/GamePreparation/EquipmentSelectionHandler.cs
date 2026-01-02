using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

#nullable enable
public class EquipmentSelectionHandler : MonoBehaviour
{
    public GameObject equipmentBoxPrefab;
    private Equipment? selectedEquipment;
    private Dictionary<string, GameObject> equipmentBoxes = new Dictionary<string, GameObject>();
    private Transform? dotArea;
    public Sprite dotSprite;
    private bool dotInitialized = false;
    private float? dotX;
    private float? dotY;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        // Initialize dots automatically cancels if already done or data not available, so no need to check double
        InitializeDots();
    }

    public void SetSlot(string? slot)
    {
        List<Equipment> equipmentList = (slot == null) ? new List<Equipment>() : Game.Instance.GetEquipmentManager().GetEquipmentForSlot(slot);
        Transform boxes = transform.Find("EquipmentBoxes");
        for (int i = 0; i < boxes.childCount; i++)
        {
            Destroy(boxes.GetChild(i).gameObject);
        }
        equipmentBoxes.Clear();
        foreach (Equipment equipment in equipmentList)
        {
            GameObject box = Instantiate(equipmentBoxPrefab, boxes);
            EquipmentBoxHandler boxHandler = box.GetComponent<EquipmentBoxHandler>();
            boxHandler.SetEquipment(equipment);
            boxHandler.SetOnClickAction(SetSelectedEquipment);
            equipmentBoxes[equipment.GetID()] = box;
        }
        if (equipmentList.Count > 0)
        {
            SetSelectedEquipment(Game.Instance.GetEquipmentManager().GetEquipment(equipmentList[0].GetID()));
        }
        else
        {
            SetSelectedEquipment(null);
        };
    }

    public void SetDotData(Transform dotArea, float? dotX, float? dotY)
    {
        this.dotArea = dotArea;
        this.dotX = dotX;
        this.dotY = dotY;
    }

    public Equipment? GetSelectedEquipment()
    {
        return selectedEquipment;
    }

    private void SetSelectedEquipment(Equipment? equipment)
    {
        if (this.selectedEquipment == equipment)
        {
            return;
        }
        if (this.selectedEquipment != null && equipmentBoxes.ContainsKey(this.selectedEquipment.GetID()))
        {
            equipmentBoxes[this.selectedEquipment.GetID()].GetComponent<EquipmentBoxHandler>().SetSelected(false);
        }
        this.selectedEquipment = equipment;
        if (equipment != null && equipmentBoxes.ContainsKey(equipment.GetID()))
        {
            equipmentBoxes[equipment.GetID()].GetComponent<EquipmentBoxHandler>().SetSelected(true);
        }
        transform.Find("CardDisplay").GetComponent<CardListDisplay>().SetCards(equipment?.GetCards() ?? new List<Card>());
        _ = UpdateText();
    }

    private async Task UpdateText() {
        TextMeshProUGUI effectText = transform.Find("OtherEffectDisplay").GetComponent<TextMeshProUGUI>();
        effectText.text = "";
        if (selectedEquipment != null)
        {
            string otherEffectText = await selectedEquipment.GetOtherEffectText();
            effectText.text = otherEffectText;
        }
    }

    private void InitializeDots()
    {
        if (dotInitialized || dotArea == null || dotX == null || dotY == null)
        {
            return;
        }
        dotInitialized = true;
        
        RectTransform parentRect = dotArea.GetComponent<RectTransform>();
        RectTransform myRect = transform.GetComponent<RectTransform>();
        
        // Calculate dot position in dotArea space
        float dotPosX = (dotX.Value - 0.5f) * parentRect.rect.width;
        float dotPosY = (dotY.Value - 0.5f) * parentRect.rect.height;
        
        // Equipment box center: top 25% of MY transform, converted to dotArea space
        float myHeight = myRect.rect.height;
        float equipmentBoxCenterYLocal = myHeight * 0.375f; // Top 25% center (37.5% up from center)
        
        // Convert my transform position to dotArea space
        Vector3 myWorldPos = myRect.TransformPoint(new Vector3(myHeight * 0.125f, equipmentBoxCenterYLocal, 0));
        Vector3 myLocalInDotArea = parentRect.InverseTransformPoint(myWorldPos);
        
        float equipmentBoxCenterY = myLocalInDotArea.y;
        float equipmentBoxStartX = myLocalInDotArea.x - myRect.rect.width * 0.5f;
        Color color = new Color(1, 1, 1, 0.3f);
        
        // Create horizontal line from equipment box center to dot x coordinate
        GameObject horizontalLine = new GameObject("HorizontalLine");
        horizontalLine.transform.SetParent(dotArea, false);
        horizontalLine.transform.SetAsFirstSibling();
        Image horizontalImage = horizontalLine.AddComponent<Image>();
        horizontalImage.color = color;
        
        RectTransform horizontalRect = horizontalLine.GetComponent<RectTransform>();
        float horizontalWidth = dotPosX - equipmentBoxStartX;
        float horizontalHeight = 2f; // Line thickness
        horizontalRect.sizeDelta = new Vector2(Mathf.Abs(horizontalWidth), horizontalHeight);
        horizontalRect.anchoredPosition = new Vector2(equipmentBoxStartX + horizontalWidth * 0.5f, equipmentBoxCenterY);
        
        // Create vertical line from horizontal line end to dot border
        GameObject verticalLine = new GameObject("VerticalLine");
        verticalLine.transform.SetParent(dotArea, false);
        verticalLine.transform.SetAsFirstSibling();
        Image verticalImage = verticalLine.AddComponent<Image>();
        verticalImage.color = color;
        
        RectTransform verticalRect = verticalLine.GetComponent<RectTransform>();
        float dotRadius = 25f; // 50x50 dot = 25px radius
        float distanceToDot = Mathf.Abs(dotPosY - equipmentBoxCenterY);
        float verticalHeight = distanceToDot - dotRadius;
        float verticalWidth = 2f; // Line thickness
        verticalRect.sizeDelta = new Vector2(verticalWidth, verticalHeight);
        
        // Position vertical line from horizontal end to dot border
        float verticalCenterY;
        if (dotPosY > equipmentBoxCenterY)
        {
            verticalCenterY = equipmentBoxCenterY + verticalHeight * 0.5f;
        }
        else
        {
            verticalCenterY = equipmentBoxCenterY - verticalHeight * 0.5f;
        }
        verticalRect.anchoredPosition = new Vector2(dotPosX, verticalCenterY);
        
        // Create center dot image
        GameObject dotObject = new GameObject("CenterDot");
        dotObject.transform.SetParent(dotArea, false);
        
        Image dotImage = dotObject.AddComponent<Image>();
        dotImage.sprite = dotSprite;
        dotImage.color = color;
        
        // Set size to 50x50 pixels
        RectTransform rectTransform = dotObject.GetComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(50f, 50f);
        rectTransform.anchoredPosition = new Vector2(dotPosX, dotPosY);
    }
}
