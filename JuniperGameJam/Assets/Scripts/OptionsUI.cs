using UnityEngine;
using UnityEngine.UI;

public class OptionsUI : MonoBehaviour
{
    [SerializeField] private Button BackButton;
    [SerializeField] private Button KeybindsButton;
    [SerializeField] private MainMenuUI MainMenuCanvas;
    [SerializeField] private KeybindsUI KeybindsCanvas;

    void Start()
    {
        BackButton.onClick.AddListener(() =>
        {
            Hide();
            MainMenuCanvas.Show();
        });

        KeybindsButton.onClick.AddListener(() =>
        {
            KeybindsCanvas.Show();
            Hide();
        });
    }

    private void Hide()
    {
        GetComponent<Canvas>().enabled = false;
        // MainMenuCanvas.Show();
    }
    public void Show()
    {
        GetComponent<Canvas>().enabled = true;
    }
}
