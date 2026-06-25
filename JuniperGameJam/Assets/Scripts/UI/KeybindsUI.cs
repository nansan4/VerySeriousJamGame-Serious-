using UnityEngine;
using UnityEngine.UI;

public class KeybindsUI : MonoBehaviour
{
    [SerializeField] private OptionsUI OptionsCanvas;
    
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
