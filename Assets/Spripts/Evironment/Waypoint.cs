using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waypoint : MonoBehaviour
{
	private void Start()
	{
		GetComponent<SpriteRenderer>().enabled = false;
	}
}
