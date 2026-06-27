using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameEndHandlerScript : MonoBehaviour
{
    [Header("Lost")]
    [SerializeField] private GameObject gameOverUI;
    [SerializeField] private Button lostRestartButton;
    [SerializeField] private TextMeshProUGUI scoreText;
    [Header("Won")]
    [SerializeField] private GameObject gameWonUI;
    [SerializeField] private Button wonRestartButton;
    [Header("Variables")]
    [SerializeField] private PlayerController playerController;
    [SerializeField] private AudioSource winSound;
    [SerializeField] private AudioClip winSoundClip;

    void Start()
    {
        GameState.Instance.OnLose.AddListener(HandleGameLost);
        GameState.Instance.OnWin.AddListener(HandleGameWon);
        lostRestartButton.onClick.AddListener(ButtonRestartGame);
        wonRestartButton.onClick.AddListener(ButtonRestartGame);
    }

    private void HandleGameLost()
    {
        PlayerAudioManager.Instance.FadeOutBGMusic(1.0f);
        UI_Manager.instance.Hide();
        GameState.Instance.OnLose.RemoveListener(HandleGameLost);
        Cursor.visible = true;
        gameOverUI.SetActive(true);
        scoreText.text = "Score: " + GameState.Instance.GameScore + "/" + GameState.Instance.maxScoreToWin;
        playerController.UnsubscribeInputActions();
    }

    private void HandleGameWon()
    {
        PlayerAudioManager.Instance.FadeOutBGMusic(0.5f);
        UI_Manager.instance.Hide();
        GameState.Instance.OnWin.RemoveListener(HandleGameWon);
        Cursor.visible = true;
        gameWonUI.SetActive(true);
        winSound.PlayOneShot(winSoundClip);
        playerController.UnsubscribeInputActions();
    }

    private void ButtonRestartGame()
    {
        SceneManager.LoadScene("MainMenuScene");
    }
}
