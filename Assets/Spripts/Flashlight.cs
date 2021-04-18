using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

[RequireComponent(typeof(FieldOfView))]
public class Flashlight : MonoBehaviour
{
	float fullBattery = 100;
	float currentBattery;

	float viewAtFull;
	float rangeAtFull;

	private FieldOfView fov;

	public float batteryDepleteRate = 0.5f;

	public FLTheshold[] flashLightThresholds;

	public event Action OnBatteryDeplete;
	public event Action OnEnemyOutOfView;

	private void Start()
	{
		fov = GetComponent<FieldOfView>();

		currentBattery = fullBattery;
		viewAtFull = fov.viewAngle;
		rangeAtFull = fov.viewRadius;

	}

	private void Update()
	{
		FreezeStoneStatue();
	}

	private void LateUpdate()
	{
		ReduceFalshlight();
	}

	void ReduceFalshlight()
	{
		currentBattery -= batteryDepleteRate * Time.deltaTime;

		OnBatteryDeplete?.Invoke();

		CheckBatteryThreshHole(currentBattery);

	}

	void CheckBatteryThreshHole(float curBattery)
	{
		for (int i = 0; i < flashLightThresholds.Length; i++)
		{
			if(curBattery <= flashLightThresholds[i].threshold && flashLightThresholds[i].atThreshold == false)
			{
				flashLightThresholds[i].atThreshold = true;
				fov.viewAngle *= flashLightThresholds[i].viewAngleModify;
				fov.viewRadius *= flashLightThresholds[i].viewRadiusModify;
			}
		}
	}

	public void RefillBattery()
	{
		currentBattery = fullBattery;
		for (int i = 0; i < flashLightThresholds.Length; i++)
		{
			flashLightThresholds[i].atThreshold = false;
		}

		fov.ResetFov();
	}

	private void FreezeStoneStatue()
	{
		if (fov.visibleTargets.Count > 0)
		{
			bool seeStatue = false;
			foreach (Transform target in fov.visibleTargets)
			{
				StoneStatue ss = target.GetComponent<StoneStatue>();
				if (ss != null)
				{
					ss.Stun();
					seeStatue = true;
				}
			}
		}
	}

	public void StunGhost()
	{
		if (fov.visibleTargets.Count > 0)
		{
			foreach (Transform target in fov.visibleTargets)
			{
				Ghost g = target.GetComponent<Ghost>();
				if (g != null)
				{
					g.Stun();
				}
			}
		}
	}

	#region GET
	public float GetCurrentBattery()
	{
		return currentBattery;
	}

	public float GetFullBattery()
	{
		return fullBattery;
	}

	#endregion

	[System.Serializable]
	public struct FLTheshold
	{
		public float threshold;
		public bool atThreshold;
		public float viewAngleModify;
		public float viewRadiusModify;
	}
}
