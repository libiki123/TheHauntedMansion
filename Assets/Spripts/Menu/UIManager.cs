using System.Collections;
using System.Collections.Generic;
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

    // Start is called before the first frame update
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateFrame(PlayerController player)
	{
        int curNumOfHeart = player.GetCurNumOfHeart();
        int numberOfHeart = player.GetNumOfHeart();
        float curBattery = player.GetFlCurrentBattery();
        float fullBattery = player.GetFlFullBattery();
        int curNumOfGem = player.GetCurNumOfGem();
        bool obtainedKey = player.IsKeyObtained();

        

		for (int i = 0; i < numberOfHeart; i++)
		{
            if (i < curNumOfHeart)
                hearts[i].sprite = fullHeart;
            else
                hearts[i].sprite = emptyHeart;
        }


        flBatteryFill.fillAmount = curBattery / fullBattery;

        gemAmountText.text = curNumOfGem + "/10";       // TO DO: get total number of gem later on

        if (obtainedKey)
            keyAmountText.text = "1/1";
    }

}
