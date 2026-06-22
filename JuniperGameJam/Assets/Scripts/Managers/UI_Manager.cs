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
    void FixedUpdate()
    {
        if (!isEndless) 
        {
            timeRemaining -= Time.fixedDeltaTime;
            SetTimerText();
        }
    }
    #endregion

    #region Stats
    public void IncrementDelivered(int num)
    {
        deliveredPackages += num;
        deliveredText.text = deliveredPackages.ToString();
    }

    public void IncrementMissed(int num)
    {
        missedPackages += num;
        missedText.text = missedPackages.ToString();
    }

    public void SetTotalPackages(int num)
    {
        totalPackages = num;
        totalText.text = $"{deliveredPackages + missedPackages}/{totalPackages}";
    }

    public void IncrementTotal(int num)
    {
        totalPackages += num;
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
}
