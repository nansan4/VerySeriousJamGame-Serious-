using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class ChangeBindingsScript : MonoBehaviour
{
    [Header("Action UI")]
    [SerializeField] private TMP_Text pauseText;

    [Header("Movement UI")]
    [SerializeField] private TMP_Text forwardText;
    [SerializeField] private TMP_Text backwardText;
    [SerializeField] private TMP_Text leftText;
    [SerializeField] private TMP_Text rightText;
    [SerializeField] private TMP_Text upText;
    [SerializeField] private TMP_Text downText;

    private InputSystem_Actions inputActions;

    private const string REBINDS_KEY = "rebinds";

    private void Start()
    {
        inputActions = InputManager.Instance.InputActions;
        RefreshUI();
    }

    #region Rebinding

    private void RebindAction(InputAction action, int bindingIndex = 0)
    {
        Debug.Log($"Starting rebind for {action.name}");

        InputActionMap actionMap = action.actionMap;

        actionMap.Disable();

        action.PerformInteractiveRebinding(bindingIndex)
            .WithCancelingThrough("<Keyboard>/escape")

            .OnComplete(operation =>
            {
                Debug.Log($"{action.name} rebound to {action.bindings[bindingIndex].effectivePath}");

                operation.Dispose();

                actionMap.Enable();

                SaveBindings();
                RefreshUI();
            })

            .OnCancel(operation =>
            {
                Debug.Log($"Rebind cancelled for {action.name}");

                operation.Dispose();

                actionMap.Enable();

                RefreshUI();
            })

            .Start();
    }

    #endregion

    #region Action Buttons

    public void RebindLaunch()
    {
        RebindAction(inputActions.Player.Launch);
    }

    public void RebindFall()
    {
        RebindAction(inputActions.Player.Fall);
    }

    public void RebindPause()
    {
        RebindAction(inputActions.Player.TogglePause);
    }

    #endregion

    #region Movement Buttons

    /*
        Move Composite

        0 = Composite
        1 = Forward
        2 = Backward
        3 = Left
        4 = Right
    */

    public void RebindMoveForward()
    {
        RebindAction(inputActions.Player.Move, 1);
    }

    public void RebindMoveBackward()
    {
        RebindAction(inputActions.Player.Move, 2);
    }

    public void RebindMoveLeft()
    {
        RebindAction(inputActions.Player.Move, 3);
    }

    public void RebindMoveRight()
    {
        RebindAction(inputActions.Player.Move, 4);
    }

    public void RebindMoveUp()
    {
        RebindAction(inputActions.Player.Launch);
    }

    public void RebindMoveDown()
    {
        RebindAction(inputActions.Player.Fall);
    }

    public void DefaultBindings()
    {
        ResetBindings();
    }

    #endregion

    #region Save / Load

    private void SaveBindings()
    {
        string rebinds =
            inputActions.asset.SaveBindingOverridesAsJson();

        PlayerPrefs.SetString(REBINDS_KEY, rebinds);
        PlayerPrefs.Save();

        Debug.Log("Bindings Saved");
    }

    private void LoadBindings()
    {
        if (!PlayerPrefs.HasKey(REBINDS_KEY))
            return;

        string rebinds =
            PlayerPrefs.GetString(REBINDS_KEY);

        inputActions.asset.LoadBindingOverridesFromJson(rebinds);

        Debug.Log("Bindings Loaded");
    }

    public void ResetBindings()
    {
        // Remove ALL runtime overrides (this restores original Input Actions asset bindings)
        inputActions.asset.RemoveAllBindingOverrides();

        // Delete saved override data so it doesn't reload next boot
        if (PlayerPrefs.HasKey(REBINDS_KEY))
        {
            PlayerPrefs.DeleteKey(REBINDS_KEY);
        }

        PlayerPrefs.Save();

        Debug.Log("Bindings Reset to Defaults");

        RefreshUI();
    }

    #endregion

    #region UI

    private void RefreshUI()
    {
        if (inputActions == null)
            return;

        pauseText.text =
            inputActions.Player.TogglePause.GetBindingDisplayString();

        forwardText.text =
            inputActions.Player.Move.bindings[1].ToDisplayString();

        backwardText.text =
            inputActions.Player.Move.bindings[2].ToDisplayString();

        leftText.text =
            inputActions.Player.Move.bindings[3].ToDisplayString();

        rightText.text =
            inputActions.Player.Move.bindings[4].ToDisplayString();

        upText.text =
            inputActions.Player.Launch.GetBindingDisplayString();
        
        downText.text =
            inputActions.Player.Fall.GetBindingDisplayString();
    }

    #endregion

    #region Helper

    public string GetBindingName(InputAction action, int bindingIndex = 0)
    {
        return action.bindings[bindingIndex].ToDisplayString();
    }

    #endregion
}