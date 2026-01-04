using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TextMeshProFontManager : MonoBehaviour
{
    public static TextMeshProFontManager Instance { get; private set; }
    
    private TMP_FontAsset currentFont;

    private void Awake()
    {
        UnityEngine.Debug.Log("Awake TextMeshProFontManager");
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public void SetDefaultFont(TMP_FontAsset font)
    {
        currentFont = font;
        ApplyFontToAllTextMeshPro();
    }

    public TMP_FontAsset GetDefaultFont()
    {
        return currentFont;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Font auf alle neuen TextMeshPro in der Szene anwenden
        if (currentFont != null)
        {
            ApplyFontToAllTextMeshPro();
        }
    }

    private void ApplyFontToAllTextMeshPro()
    {
        foreach (TextMeshProUGUI textUI in FindObjectsByType<TextMeshProUGUI>(FindObjectsInactive.Include, FindObjectsSortMode.None))
        {
            textUI.font = currentFont;
        }
    }
}