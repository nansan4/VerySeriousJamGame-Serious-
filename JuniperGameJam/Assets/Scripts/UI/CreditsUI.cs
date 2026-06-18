using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CreditsUI : MonoBehaviour
{
    [SerializeField] private Button BackButton;
    [SerializeField] private MainMenuUI MainMenu;

    void Start()
    {
        BackButton.onClick.AddListener(() =>
        {
            Hide();
            MainMenu.Show();
        });
    }

    private void Hide()
    {
        GetComponent<Canvas>().enabled = false;
    }
    public void Show()
    {
        GetComponent<Canvas>().enabled = true;
    }
}
