using UnityEngine;
using UnityEngine.UI;

public class OptionsUI : MonoBehaviour
{
    [SerializeField] private Button BackButton;
    [SerializeField] private Button KeybindsButton;
    [SerializeField] private Button AudioButton;
    [SerializeField] private MainMenuUI MainMenuCanvas;
    [SerializeField] private PauseMenu pauseMenu;
    [SerializeField] private KeybindsUI KeybindsCanvas;
    [SerializeField] private AudioMenuUI audioMenu;

    void Start()
    {
        BackButton.onClick.AddListener(() =>
        {
            Hide();
            if (pauseMenu == null) MainMenuCanvas.Show();
            else if (MainMenuCanvas == null) pauseMenu.Show();
            else Debug.LogError("What scene are we in?");
        });

        KeybindsButton.onClick.AddListener(() =>
        {
            KeybindsCanvas.Show();
            Hide();
        });

        AudioButton.onClick.AddListener(() =>
        {
            audioMenu.Show();
            Hide();
        });
    }

    private void Hide()
    {
        GetComponent<Canvas>().enabled = false;
        GetComponent<GraphicRaycaster>().enabled = false;
    }
    public void Show()
    {
        GetComponent<Canvas>().enabled = true;
        GetComponent<GraphicRaycaster>().enabled = true;
    }
}
