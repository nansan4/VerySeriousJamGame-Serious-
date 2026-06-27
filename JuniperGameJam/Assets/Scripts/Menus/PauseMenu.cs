using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [Header("Pause Menu Vars")]
    [SerializeField] private Canvas canvas;
    [SerializeField] private GraphicRaycaster graphicRaycaster;
    [SerializeField] private PlayerController playerController;
    [Header("External Vars")]
    [SerializeField] private OptionsUI optionsCanvas;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameState.Instance.OnGamePaused.AddListener(Show);
        GameState.Instance.OnGameResumed.AddListener(Hide);
        Hide();
    }

    public void Show()
    {
        canvas.enabled = true;
        graphicRaycaster.enabled = true;
    }

    public void Hide()
    {
        canvas.enabled = false;
        graphicRaycaster.enabled = false;
    }

    #region Button Functions
    public void OnResumeButtonPressed()
    {
        GameState.Instance.SetGamePaused(false);
        GameState.Instance.SetGameStatus(GameStatus.InProgress);
        playerController.SwitchActionMap(playerController.playerInputActions.Player, false);
    }

    public void OnOptionsButtonPressed()
    {
        Hide();
        optionsCanvas.Show();
    }

    public void OnQuitButtonPressed()
    {
        // GameState.Instance.SetGamePaused(false);
        // GameState.Instance.SetGameStatus(GameStatus.InProgress);
        //Time.timeScale = 1;
        //Cursor.visible = true;
        //Cursor.lockState = CursorLockMode.Confined;
        //GameState.Instance.SetGamePaused(false);
        //GameState.Instance.SetGameStatus(GameStatus.Initializing);
        SceneManager.LoadScene("MainMenuScene");
    }

    #endregion
}
