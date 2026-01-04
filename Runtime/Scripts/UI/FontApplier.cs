using TMPro;
using UnityEngine;

public class FontApplier : MonoBehaviour
{
    private void OnEnable()
    {
        // Wenn dieses UI-Element aktiviert wird, 
        // gib allen TextMeshPro-Kindern die aktuelle Font
        if (TextMeshProFontManager.Instance != null)
        {
            foreach (TextMeshProUGUI textUI in GetComponentsInChildren<TextMeshProUGUI>(includeInactive: true))
            {
                textUI.font = TextMeshProFontManager.Instance.GetDefaultFont();
            }
        }
    }
}