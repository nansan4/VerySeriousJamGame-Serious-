#region Imports
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Events; //gives access to UnityEvent type
using UnityEngine.SceneManagement; //give us access to SceneManager
#endregion

/// <summary>
/// The GameState is an overarching manager that handles rules and information about the broader state of the game, 
/// like player lives, match wins, money, etc. Anything that may need to be saved in a save system or transfer between levels
/// should be handled here
/// </summary>
public class GameState : MonoBehaviour
{
    #region Singleton
    //Singleton Pattern

    private static GameState _instance = null; //static/global reference to the single instance of the object because singleton pattern
    public static GameState Instance { get { return _instance; } } //safe way for other objects to reference GameManager without changing the ref
    #endregion

    #region Time Scale
    private float _defaultTimeScale = 1.0f; //default time scale of the game
    public float DefaultTimeScale {  get { return _defaultTimeScale; } }
    public float usedTimeScale = 1.0f; //the time scale that can be edited, used for slow-motion and unpausing
    #endregion

    #region Pause State
    protected bool isPaused = false;
    public bool IsPaused {  get { return isPaused; } }
    #endregion

    #region References

    [SerializeField] private CinemachineImpulseSource _impulseSource;
    #endregion

    #region Game Status
    [SerializeField] protected GameStatus currentGameStatus = GameStatus.Initializing; //represents progress status of the game
    public GameStatus CurrentGameStatus { get { return currentGameStatus; } }
    #endregion

    //declare game-specific info HERE(lives, timer, etc.)
    
    #region Game Values
    //[SerializeField] private AudioSource playerDamageSound;
    [SerializeField] private float timeSpeedUpMultiplier = 1.0f;
    [Header("Game Stats")]
    [SerializeField] private int maxScoreToWin = 100;
    private int _currentScore = 0;

    [SerializeField] private bool isEndless;
    [SerializeField] private float gameDuration = 120f;

    public int GameScore { get { return _currentScore; } }
    #endregion

    #region Difficulty Params
    [Header("Difficulty Params")]
    [SerializeField] private float _boxMalfunctionBaseChance_Normal = 1f;
    [SerializeField] private float _boxMalfunctionBaseChance_Hard = 1f;

    [SerializeField] private float _boxInvalidityBaseTime_Normal = 60f;
    [SerializeField] private float _boxInvalidityBaseTime_Hard = 30f;

    [SerializeField] private float _initialSpawnDelayBaseTime_Normal = 5f;
    [SerializeField] private float _initialSpawnDelayBaseTime_Hard = 5f;

    //this is what the game uses, set based on difficulty
    private float _boxMalfunctionChance;
    private float _boxInvalidityTime;
    [SerializeField] private float _initialSpawnDelayTime; //serializefield for debugging purposes, but should be set based on difficulty
    #endregion

    #region DifficultyGetters
    public float BoxMalfunctionChance { get { return  _boxMalfunctionChance; } }
    public float BoxInvalidityTime { get { return _boxInvalidityTime; } }
    public float InitialSpawnDelayTime { get  { return _initialSpawnDelayTime; } }

    #endregion

    #region Events
    //invoke whenever important info in GameState changes, observers (listeners) can execute code when invoked
    public UnityEvent OnGamePaused;
    public UnityEvent OnGameResumed;

    public UnityEvent OnLose;
    public UnityEvent OnWin;

    //bring along GameStatus info with UnityEvent<T>
    public UnityEvent<GameStatus> OnGameStatusChanged;
    #endregion

    #region Unity Lifecycle
    private void Awake()
    {
        #region Persistent Singleton

        if (_instance == null) //if an instance of the object doesnt already exist
        {
            _instance = this;

            DontDestroyOnLoad(gameObject); //we want this object to persist between scenes, so dont destroy it when loading a new scene
        }
        else //if an instance already exists and its not this one
        {
            Destroy(gameObject); //gameObject points to the object the script is on
        }

        #endregion

        //any time we load into a new scene, call OnSceneLoaded()
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void Start()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;

        //for(int i = 0; i < maxHealth; i++)
        //{
        //    OnPlayerHealed?.Invoke();
        //}
        OnGameStatusChanged.AddListener(GameStatusChanged);

        UI_Manager.instance.SetTotalPackages(maxScoreToWin);
        UI_Manager.instance.SetTimeRemaining(gameDuration);
    }

    private void GameStatusChanged(GameStatus gameStatus)
    {
        if (gameStatus == GameStatus.Initializing)
        {
            UI_Manager.instance.SetTotalPackages(maxScoreToWin);
            if (!isEndless)
            {
                UI_Manager.instance.SetTimeRemaining(gameDuration);
            }
            else
            {
                UI_Manager.instance.SetTimeRemaining(-1f);
            }
        }
    }
    #endregion

    #region Scene Handling
    public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == Globals.MAIN_MENU_SCENE_NAME) //if this is the main menu scene, reset GameState
        {
            ResetGameState();
        }
        Cursor.visible = true;
    }
    #endregion

    #region State Control
    public void SetGamePaused(bool paused)
    {
        isPaused = paused;

        if (isPaused)
        {
            //set timeScale to 0, meaning anything based on time (physics, anims, etc.) will not occur until timeScale is set back to 1
            Time.timeScale = 0f;

            //In C#, ? = null-coalescing operator; only call func if operand (in this case event) isn't null
            //invoking event to notify relevant observers
            OnGamePaused?.Invoke();
        }
        else
        {
            //set time scale back to normal
            Time.timeScale = usedTimeScale;

            //same as above
            OnGameResumed?.Invoke();
        }
    }

    public bool SetGameStatus(GameStatus newGameStatus)
    {
        if (newGameStatus == currentGameStatus)
        {
            return false; //don't switch into the same status
        }

        currentGameStatus = newGameStatus;
        OnGameStatusChanged?.Invoke(currentGameStatus);//invoke event and pass along relevant info

        return true; //succesfully updated game state
    }
    #endregion

    #region Reset
    public void ResetGameState() //returns all values in GameState to "default"
    {
        Debug.Log("Resetting game state");

        //clear all events
        OnGamePaused?.RemoveAllListeners();
        OnGameResumed?.RemoveAllListeners();
        OnGameStatusChanged?.RemoveAllListeners();
        OnLose?.RemoveAllListeners();
        OnWin?.RemoveAllListeners();

        //reset state info
        SetGamePaused(false);
        SetGameStatus(GameStatus.Initializing);

        //reset other info here (lives, timer, etc.)
    }
    #endregion

    #region Camera
    public void ShakeCamera()
    {
        _impulseSource.GenerateImpulse();
    }
    #endregion

    #region Time Control
    public IEnumerator ChangeGameTimeScaleRoutine(float duration, float newTimeScale, bool fade = false)
    {
        //Debug.Log("started changing time");

        if (fade)
        {
            while (usedTimeScale > newTimeScale)
            {
                usedTimeScale -= timeSpeedUpMultiplier * Time.deltaTime;
                Time.timeScale = usedTimeScale;
                yield return null;
            }

            usedTimeScale = newTimeScale;
            Time.timeScale = usedTimeScale;

        }
        else
        {
            usedTimeScale = newTimeScale;
            Time.timeScale = usedTimeScale;

        }

        yield return new WaitForSecondsRealtime(duration);


        if (fade)
        {
            while(usedTimeScale < 1)
            {
                usedTimeScale += timeSpeedUpMultiplier * Time.deltaTime;
                Time.timeScale = usedTimeScale;
                yield return null;
            }

            usedTimeScale = DefaultTimeScale;
            Time.timeScale = usedTimeScale;

        }
        else
        {
            usedTimeScale = DefaultTimeScale;
            Time.timeScale = usedTimeScale;

        }

        //Debug.Log("time has been changed");
    }

    public void ChangeGameTimeScale(float timeScale)
    {
        usedTimeScale = timeScale;
        Time.timeScale = usedTimeScale;


        Debug.Log("time has been succesfully changed!");
    }
    #endregion

    #region Score
    public void IncrementScore()
    {
        _currentScore++;
        Debug.Log("GameState score: " + _currentScore);

        if(_currentScore >= maxScoreToWin)
        {
            //start win sequence
            OnWin?.Invoke();
        }
    }

    public void DecrementScore()
    {
        _currentScore--;
    }
    #endregion
}

#region Enums
//track status with enum because names are easier than numbers
public enum GameStatus
{
    Initializing,   //init
    InProgress,     //playing game
    Paused,
    PlayerWon,      //game over
    PlayerLost      //game over
}
#endregion
