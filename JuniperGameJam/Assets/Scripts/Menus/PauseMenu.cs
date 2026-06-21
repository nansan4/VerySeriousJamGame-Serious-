using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private Canvas canvas;
    [SerializeField] private GraphicRaycaster graphicRaycaster;
    [SerializeField] private PlayerController playerController;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameState.Instance.OnGamePaused.AddListener(Show);
        GameState.Instance.OnGameResumed.AddListener(Hide);
        Hide();
    }

    private void Show()
    {
        canvas.enabled = true;
        graphicRaycaster.enabled = true;
    }

    private void Hide()
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

    public void OnQuitButtonPressed()
    {
        // Application.Quit();
    }

    #endregion
}
