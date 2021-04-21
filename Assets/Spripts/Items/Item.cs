using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    GameObject player;
    PlayerController playerCl;
    Collider2D itemCollier;

    bool canPickup = true;
    float ignoreTimer;

    public Item_SO item;
    public float ignoreTimerSet = 2f;

	private void Start()
	{
        player = GameObject.FindGameObjectWithTag("Player");
        playerCl = player.GetComponent<PlayerController>();
        itemCollier = GetComponent<Collider2D>();

        ignoreTimer = ignoreTimerSet;
	}

	private void Update()
	{
		if (!canPickup)
		{
            ignoreTimer -= Time.deltaTime;

            itemCollier.enabled = false;

            if (ignoreTimer <= 0)
			{
                itemCollier.enabled = true;
                ignoreTimer = ignoreTimerSet;
                canPickup = true;
            }
        }
	}

	void Pickup()
	{
        switch (item.type)
        {
            case ItemType.HEART:
                if(playerCl.GetCurNumOfHeart() < playerCl.GetNumOfHeart())
				{
                    playerCl.GiveHeart();
                    Destroy(gameObject);
				}
                canPickup = false;
                break;
            case ItemType.BATTERY:
                if (playerCl.GetFlCurrentBattery() < playerCl.GetFlFullBattery())
				{
                    playerCl.RefillBattery();
                    Destroy(gameObject);
                }
                break;
            case ItemType.GEM:
                ObjectiveManager.Instance.curGemCount++;
                GameManager.Instance.OnPLayerObtainItem();
                Destroy(gameObject);
                break;
            case ItemType.KEY:
                playerCl.GiveKey();
                Destroy(gameObject);
                break;
        }
    }


    private void OnCollisionEnter2D(Collision2D collision)
	{
        if (collision.gameObject.tag == "Player")
            Pickup();
	}

}
