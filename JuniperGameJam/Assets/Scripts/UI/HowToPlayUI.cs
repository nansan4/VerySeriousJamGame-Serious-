using UnityEngine;
using UnityEngine.UI;

public class HowToPlayUI : MonoBehaviour
{
    [SerializeField] private Button backButton;


    private void Start()
    {
        backButton.onClick.AddListener(() =>
        {
            
            Hide();
        });
    }

    private void Hide()
    {
        // gameObject.SetActive(false);
        GetComponent<Canvas>().enabled = false;
    }

    
}
