using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Manager<GameManager>
{
    public enum GameState { PREGAME, RUNNING, PAUSED, POSTGAME }

    private PlayerController playerController;
    private PlayerController player         // We cant do this in Start() because the hero appear after the Game Manager
    {                                   // Also we want to keep the hero reference here to avoid calling find<herocontroller> in others (exspensive)
        get                             // this called lazy initialize 
        {
            if (null == playerController)
            {
                playerController = FindObjectOfType<PlayerController>();
            }

            return playerController;
        }
    }

    private GameState _currentGameState = GameState.RUNNING;           ///////////////// CHANGE THIS TO PREGAME IF WANT TO USE MAIN MENU/////////////////
    public GameState CurrentGameState
    {
        get { return _currentGameState; }
        set { _currentGameState = value; }      // value mean whatever value that being set to the currentState 
    }

    public Vector2 lastCheckpoint;

    public event Action<GameManager.GameState, GameManager.GameState> OnGameStateChanged;

	private void Start()
	{
        //Time.timeScale = 0.0f;        // WHEN THERES A MAINMENU
    }

	void Update()
    {
        if (_currentGameState == GameState.PREGAME)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    void UpdateState(GameState state)
    {
        GameState previousGameState = _currentGameState;        // Save to previous game state
        _currentGameState = state;                              // Change to the new game state

        switch (_currentGameState)
        {
            case GameState.PREGAME:     // Initialize any systems that need to be reset
                Time.timeScale = 0.0f;
                break;

            case GameState.RUNNING:     //  Unlock player, enemies and input in other systems, update tick if you are managing time
                Time.timeScale = 1.0f;
                break;

            case GameState.PAUSED:      // Pause player, enemies etc, Lock other input in other systems
                Time.timeScale = 0.0f;
                break;

            default:
                break;
        }

        OnGameStateChanged.Invoke(_currentGameState, previousGameState);
    }

    public void GameStart()
	{
        UpdateState(GameState.RUNNING);
    }

    public void TogglePause()
    {
        UpdateState(_currentGameState == GameState.RUNNING ? GameState.PAUSED : GameState.RUNNING);
    }

    public IEnumerator EndGame()
    {
        UpdateState(GameState.POSTGAME);
        yield return new WaitForSeconds(1.5f);
        SceneManager.LoadScene("Credit");
    }

    #region Callbacks
    public void OnPLayerBatteryDeplete()
	{
        UIManager.Instance.UpdateFrame(player);
	}

    public void OnPlayerDamaged()
	{
        UIManager.Instance.UpdateFrame(player);
	}

    public void OnPLayerObtainItem()
    {
        UIManager.Instance.UpdateFrame(player);
    }

	#endregion
}
