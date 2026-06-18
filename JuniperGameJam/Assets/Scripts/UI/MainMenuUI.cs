using System.Collections;
//using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private Button PlayButton;
    [SerializeField] private Button OptionsButton;
    [SerializeField] private Button QuitButton;
    [SerializeField] private OptionsUI OptionsCanvas;
    [SerializeField] private Button CreditsButton;
    [SerializeField] private CreditsUI CreditsCanvas;

    //[SerializeField] private CinemachineCamera _camera;
    //[SerializeField] private CinemachineCamera _camera2;

    [SerializeField] private float fadeSpeed = 0.1f;

    private CanvasGroup canvasGroup;
    public static MainMenuUI Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            //DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }
    void Start()
    {
        canvasGroup = gameObject.GetComponent<CanvasGroup>();
        PlayButton.onClick.AddListener(() =>
        {
            giveCameraTwoPriority();
            StartCoroutine(FadeOut());

        });

        OptionsButton.onClick.AddListener(() =>
        {
            OptionsCanvas.Show();
            Hide();
        });

        QuitButton.onClick.AddListener(() =>
        {
            Application.Quit();
            if (Application.isEditor)
            {
               EditorApplication.isPlaying = false;
            }
        });

        CreditsButton.onClick.AddListener(() =>
        {
            CreditsCanvas.Show();
            Hide();
        });
    }

    private IEnumerator FadeOut()
    {
        while (canvasGroup.alpha > 0.01f)
        {
            canvasGroup.alpha = Mathf.Lerp(
                canvasGroup.alpha,
                0,
                fadeSpeed * Time.deltaTime
            );

            yield return null;
        }

        canvasGroup.alpha = 0;
        Hide();
    }
    public IEnumerator FadeIn()
    {
        while (canvasGroup.alpha < 0.99f)
        {
            canvasGroup.alpha = Mathf.Lerp(
                canvasGroup.alpha,
                1,
                fadeSpeed * Time.deltaTime
            );

            yield return null;
        }

        canvasGroup.alpha = 1;
        
    }

    public void giveCameraOnePriority()
    {
        //_camera.Priority = 1;
        //_camera2.Priority = 0;
    }

    public void giveCameraTwoPriority()
    {
        //_camera.Priority = 0;
        //_camera2.Priority = 1;
    }
    private void Hide()
    {
        // gameObject.SetActive(false);
        GetComponent<Canvas>().enabled = false;
    }

    public void Show()
    {
        // gameObject.SetActive(true);
        GetComponent<Canvas>().enabled = true;
    }
}
