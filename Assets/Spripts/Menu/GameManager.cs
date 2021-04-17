using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Manager<GameManager>
{

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

    public Vector2 lastCheckpoint;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

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
}
