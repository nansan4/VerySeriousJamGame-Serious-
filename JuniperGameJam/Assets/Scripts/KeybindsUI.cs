using UnityEngine;

public class KeybindsUI : MonoBehaviour
{
    [SerializeField] private OptionsUI OptionsCanvas;
    
    public void Show()
    {
        GetComponent<Canvas>().enabled = true;
    }
    public void Hide()
    {
        GetComponent<Canvas>().enabled = false;
        OptionsCanvas.Show();
    }
}
