using System.Collections;
//using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

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

    [SerializeField] private float fadeSpeed = 0.05f;

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
    }

    private IEnumerator FadeOut()
    {
        while (canvasGroup.alpha > 0.01f)
        {
            canvasGroup.alpha = Mathf.Lerp(canvasGroup.alpha, 0f, fadeSpeed);

            yield return null;
        }

        canvasGroup.alpha = 0;
        Hide();

        StartCoroutine(LoadSceneRoutine());
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

    private IEnumerator LoadSceneRoutine()
    {
        SceneManager.LoadScene(Globals.GAMEPLAY_SCENE_NAME);
        yield return null;
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
