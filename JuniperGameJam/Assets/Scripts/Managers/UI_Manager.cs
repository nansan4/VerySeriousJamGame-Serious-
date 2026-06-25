using TMPro;
using UnityEngine;

public class UI_Manager : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private Canvas uiCanvas;
    [SerializeField] private TextMeshProUGUI deliveredText;
    [SerializeField] private TextMeshProUGUI missedText;
    [SerializeField] private TextMeshProUGUI totalText;
    [SerializeField] private TextMeshProUGUI timerText;

    [Header("Stats")]
    [SerializeField] private int deliveredPackages;
    [SerializeField] private int missedPackages;
    [SerializeField] private int totalPackages;
    [SerializeField] private float timeRemaining;
    [SerializeField] private bool isEndless;
    private bool timerWarningPlayed = false;

    #region Singleton
    public static UI_Manager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    #endregion

    #region Unity Lifecycle
    void Start()
    {
        GameState.Instance.OnGamePaused.AddListener(Hide);
        GameState.Instance.OnGameResumed.AddListener(Show);
    }

    void FixedUpdate()
    {
        if (!isEndless) 
        {
            timeRemaining -= Time.fixedDeltaTime;
            SetTimerText();
        }
        if (timeRemaining <= 10f && !isEndless)
        {
            if (!timerWarningPlayed)
            {
                PlayerAudioManager.Instance.PlayTimerWarningSFX();
                timerWarningPlayed = true;
            }
        }
        if (timeRemaining <= 0 && !isEndless) //TODO: End the Game
        {
            // GameState.Instance.SetGameStatus(GameStatus.Lost);
        }
    }
    #endregion

    #region Stats
    public void IncrementDelivered(int num)
    {
        deliveredPackages += num;
        deliveredText.text = deliveredPackages.ToString();
        PlayerAudioManager.Instance.PlayDeliveredSFX();
        UpdateTotalText();
    }

    public void IncrementMissed(int num)
    {
        missedPackages += num;
        missedText.text = missedPackages.ToString();
        PlayerAudioManager.Instance.PlayFailedSFX();
        UpdateTotalText();
    }

    public void SetTotalPackages(int num)
    {
        totalPackages = num;
        UpdateTotalText();
    }

    public void IncrementTotal(int num)
    {
        totalPackages += num;
        UpdateTotalText();
    }

    private void UpdateTotalText()
    {
        totalText.text = $"{deliveredPackages + missedPackages}/{totalPackages}";
    }
    #endregion

    #region Timer
    public void SetTimeRemaining(float time)
    {
        timeRemaining = time;
        if (timeRemaining == -1)
        {
            isEndless = true;
        }
        SetTimerText();
    }
    private void SetTimerText()
    {
        if (!isEndless)
        {
            int totalSeconds = Mathf.Max(0, Mathf.CeilToInt(timeRemaining));
            int minutes = totalSeconds / 60;
            int seconds = totalSeconds % 60;

            timerText.text = $"{minutes:00}:{seconds:00}";
        }
        else
        {
            timerText.text = "∞";
        }
    }
    #endregion

    #region Hide/Show
    public void Show()
    {
        uiCanvas.enabled = true;
    }

    public void Hide()
    {
        uiCanvas.enabled = false;
    }
    #endregion
}
