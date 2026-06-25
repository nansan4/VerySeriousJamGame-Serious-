using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    #region Variables

    [Header("Player Input")]
    public InputSystem_Actions playerInputActions; // This is the object that listens for inputs at the hardware level
    private Vector2 movementInput;

    [Header("Component / Object References")]
    [SerializeField] private BaseMovement baseMovement;

    #endregion

    #region Unity Functions

    // Awake is called before Start() when an object is created or when the level is loaded
    private void Awake()
    {
        // Set up our player actions in code
        // This class name is based on what you named your .inputactions asset
        if (InputManager.Instance != null)
        {
            playerInputActions = InputManager.Instance.InputActions;
        }
        else
        {
            playerInputActions = new InputSystem_Actions();
            Debug.LogError("InputManager does not exist in the scene! Please add an InputManager object to the scene.");
        }
    }

    private void Start()
    {
        // GameState callback listeners (call in Start to ensure GameState.Instance is ready)
        GameState.Instance.OnGamePaused.AddListener(OnGamePausedReceived);
        GameState.Instance.OnGameResumed.AddListener(OnGameResumedReceived);
        GameState.Instance.SetGameStatus(GameStatus.InProgress); // Set the game status to InProgress at the start of the game
    }

    private void OnEnable()
    {
        // Here we can subscribe functions to our
        // input actions to make code occur when
        // our input actions occur
        SubscribeInputActions();

        // We need to enable our "Player" action map so Unity will listen for our input
        SwitchActionMap(playerInputActions.Player);
    }

    private void OnDisable()
    {
        // Disable all action maps
        SwitchActionMap();

        // Here we can unsubscribe our functions
        // from our input actions so our object
        // doesn't try to call functions after
        // it is destroyed
        UnsubscribeInputActions();
    }

    private void OnDestroy()
    {
        // GameState callback listener cleanup
        GameState.Instance.OnGamePaused.RemoveListener(OnGamePausedReceived);
        GameState.Instance.OnGameResumed.RemoveListener(OnGameResumedReceived);
    }

    #endregion

    #region Custom Functions

    #region Input Handling

    private void SubscribeInputActions()
    {
        // Here we can bind our input actions to functions
        playerInputActions.Player.Move.started += MoveAction;
        playerInputActions.Player.Move.performed += MoveAction;
        playerInputActions.Player.Move.canceled += MoveAction;

        playerInputActions.Player.Launch.performed += LaunchActionPerformed;
        playerInputActions.Player.Launch.canceled += LaunchActionCanceled;
        playerInputActions.Player.Fall.performed += FallActionPerformed;
        playerInputActions.Player.Fall.canceled += FallActionCancelled;

        playerInputActions.Player.TogglePause.performed += TogglePauseActionPerformed;
        playerInputActions.UI.TogglePause.performed += TogglePauseActionPerformed;

    }

    public void UnsubscribeInputActions()
    {
        // It is important to unbind and actions that we bind
        // when our object is destroyed, or this can cause issues
        playerInputActions.Player.Move.started -= MoveAction;
        playerInputActions.Player.Move.performed -= MoveAction;
        playerInputActions.Player.Move.canceled -= MoveAction;

        playerInputActions.Player.Launch.performed -= LaunchActionPerformed;
        playerInputActions.Player.Launch.canceled -= LaunchActionCanceled;
        playerInputActions.Player.Fall.performed -= FallActionPerformed;
        playerInputActions.Player.Fall.canceled -= FallActionCancelled;

        playerInputActions.Player.TogglePause.performed -= TogglePauseActionPerformed;
        playerInputActions.UI.TogglePause.performed -= TogglePauseActionPerformed;
    }

    /// <summary>
    /// Helper function to switch to a particular action map
    /// in our player's Input Actions Asset.
    /// </summary>
    /// <param name="newActionMap">The ActionMap we want to switch to, or null to disable all ActionMaps.</param>
    /// <param name="showCursor">Whether or not to show/unlock the cursor.</param>
    public void SwitchActionMap(InputActionMap newActionMap = null, bool showCursor = false)
    {
        // Disable all action maps
        playerInputActions.Player.Disable();
        playerInputActions.UI.Disable();
        playerInputActions.Paused.Disable();

        // Enable the new action map (if it exists) and configure the cursor
        if (newActionMap != null) { newActionMap.Enable(); }
        Cursor.visible = showCursor;
        Cursor.lockState = showCursor ? CursorLockMode.None : CursorLockMode.Locked;
    }

    #endregion

    #region Input Actions

    private void MoveAction(InputAction.CallbackContext context)
    {
        // Read in the Vector2 of our player input.
        movementInput = context.ReadValue<Vector2>();

        //Debug.Log("The player is trying to move: " + movementInput);

        baseMovement.SetMovementInput(movementInput);
    }

    private void LaunchActionPerformed(InputAction.CallbackContext context)
    {
        //Debug.Log("The player is trying to launch!");

        baseMovement.Launch();
    }

    private void LaunchActionCanceled(InputAction.CallbackContext context)
    {
        //Debug.Log("The player is trying to Stop Launching!");

        baseMovement.CancelLaunch();
    }

    private void FallActionPerformed(InputAction.CallbackContext context)
    {
        baseMovement.Fall();
    }

    private void FallActionCancelled(InputAction.CallbackContext context)
    {
        baseMovement.CancelFall();
    }

    private void TogglePauseActionPerformed(InputAction.CallbackContext context)
    {
        Debug.Log("The player is trying to pause/unpause!");

        GameManager.Instance.TogglePause();
    }

    #endregion

    #region Pause Callbacks

    private void OnGamePausedReceived()
    {
        SwitchActionMap(playerInputActions.UI, true);
        GameState.Instance.SetGameStatus(GameStatus.Paused);
    }

    private void OnGameResumedReceived()
    {
        SwitchActionMap(playerInputActions.Player, false);
        GameState.Instance.SetGameStatus(GameStatus.InProgress);
    }

    #endregion

    #endregion
}
