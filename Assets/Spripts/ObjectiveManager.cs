using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectiveManager : Manager<ObjectiveManager>
{
	public int totalGem;
	public int curGemCount = 0;

	GameObject key;
	private void Start()
	{
		GameObject[] gems = GameObject.FindGameObjectsWithTag("Gem");
		key = GameObject.FindGameObjectWithTag("Key");

		key.SetActive(false);
		totalGem = gems.Length;
	}

	private void Update()
	{
		if(curGemCount == totalGem)
		{
			key.SetActive(true);
		}
	}
}
