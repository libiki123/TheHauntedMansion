using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zone : MonoBehaviour
{
    public float zoneRadius;
    public Transform[] waypointsInZone;

	private void OnDrawGizmos()
	{
		Gizmos.DrawWireSphere(transform.position, zoneRadius);
	}
}
