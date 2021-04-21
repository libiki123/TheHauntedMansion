using System;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : Manager<UIManager>
{
    [SerializeField] private Image[] hearts;
    [SerializeField] private Sprite fullHeart;
    [SerializeField] private Sprite emptyHeart;

    [SerializeField] private Image flBatteryFill;

    [SerializeField] private Text keyAmountText;
    [SerializeField] private Text gemAmountText;

    [SerializeField] private MainMenu mainMenu;
    [SerializeField] private PauseMenu pauseMenu;
    [SerializeField] private GameObject frame;
    
    void Start()
    {
        mainMenu.OnMainMenuFadeComplete += HandleMainMenuFadeComplete;
        GameManager.Instance.OnGameStateChanged += HandleGameStateChanged;
    }

    private void Update()
    {
        if (GameManager.Instance.CurrentGameState != GameManager.GameState.PREGAME)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            GameManager.Instance.GameStart();
        }
    }

    public void UpdateFrame(PlayerController player)
	{
        int curNumOfHeart = player.GetCurNumOfHeart();
        int numberOfHeart = player.GetNumOfHeart();
        float curBattery = player.GetFlCurrentBattery();
        float fullBattery = player.GetFlFullBattery();
        bool obtainedKey = player.IsKeyObtained();

        

		for (int i = 0; i < numberOfHeart; i++)
		{
            if (i < curNumOfHeart)
                hearts[i].sprite = fullHeart;
            else
                hearts[i].sprite = emptyHeart;
        }


        flBatteryFill.fillAmount = curBattery / fullBattery;

        gemAmountText.text = ObjectiveManager.Instance.curGemCount + "/" + ObjectiveManager.Instance.totalGem;       // TO DO: get total number of gem later on

        if (obtainedKey)
            keyAmountText.text = "1/1";
    }

    void HandleGameStateChanged(GameManager.GameState currentState, GameManager.GameState previousState)
    {
        pauseMenu.gameObject.SetActive(currentState == GameManager.GameState.PAUSED);
        bool showUnitFrame = currentState == GameManager.GameState.RUNNING || currentState == GameManager.GameState.PAUSED;
        frame.SetActive(showUnitFrame);
    }

    void HandleMainMenuFadeComplete(bool fadeOut)
    {
        mainMenu.gameObject.SetActive(false);
        frame.SetActive(true);
    }

}
