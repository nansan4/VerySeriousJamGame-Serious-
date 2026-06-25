using UnityEngine;
using UnityEngine.UI;

public class KeybindsUI : MonoBehaviour
{
    [SerializeField] private OptionsUI OptionsCanvas;
    [SerializeField] private Button backButton;
    [SerializeField] private Button defaultButton;
    private ChangeBindingsScript changeBindingsScript;

    void Start()
    {
        changeBindingsScript = GetComponent<ChangeBindingsScript>();
        defaultButton.onClick.AddListener(changeBindingsScript.ResetBindings);
        backButton.onClick.AddListener(Hide);
    }
    
    public void Show()
    {
        GetComponent<Canvas>().enabled = true;
        GetComponent<GraphicRaycaster>().enabled = true;
    }
    public void Hide()
    {
        GetComponent<Canvas>().enabled = false;
        GetComponent<GraphicRaycaster>().enabled = false;
        OptionsCanvas.Show();
    }
}
