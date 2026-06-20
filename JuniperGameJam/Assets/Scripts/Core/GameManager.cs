using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// The GameManager is a scene-specific manager that sets the rules for gameplay in that scene.
/// If you want to call functionality in the GameState, create and call a corresponding function in the GameManager
/// </summary>
public class GameManager : MonoBehaviour
{
    //Singleton Pattern
    private GameState gameState;

    private static GameManager _instance = null; //static/global reference to the single instance of the object because singleton pattern
    public static GameManager Instance {  get { return _instance; } } //safe way for other objects to reference GameManager without changing the ref


    private void Awake()
    {
        #region Singleton

        if (_instance == null) //if an instance of the object doesnt already exist
        {
            _instance = this;
        }
        else //if an instance already exists and its not this one
        {
            Destroy(gameObject); //gameObject points to the object the script is on
        }

        #endregion
    }

    private void Start()
    {
        gameState = GameState.Instance;

        StartCoroutine(InitialSpawnRoutine());
    }

    public void PauseGame()
    {
        gameState.SetGamePaused(true);
    }

    public void ResumeGame()
    {
        gameState.SetGamePaused(false);
    }

    public void TogglePause() //helper func
    {
        if (gameState.IsPaused)
        {
            ResumeGame();
        }
        else
        {
            PauseGame();
        }
    }

    public void ChangeTimeScale(float newTimeScale)
    {
        Debug.Log($"hopefully changing time - GM: {newTimeScale}");

        gameState.ChangeGameTimeScale(newTimeScale);
    }

    public void ShakeCamera()
    {
        gameState.ShakeCamera();
    }

    public void SetGameStatus(GameStatus newStatus)
    {
        Debug.Log($"Setting status to: {newStatus}");
        gameState.SetGameStatus(newStatus);
    }

    private IEnumerator InitialSpawnRoutine()
    {
        yield return new WaitForSeconds(gameState.InitialSpawnDelayTime);
        Debug.Log("initially spawning boxes");
        DeliveryManager.Instance.SpawnDeliverables(true);
    }

    public void IncrementScore()
    {
        gameState.IncrementScore();
    }

    public void DecrementScore()
    {
        gameState.DecrementScore();
    }
}
