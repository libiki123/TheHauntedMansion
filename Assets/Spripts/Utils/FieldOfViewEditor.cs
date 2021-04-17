  using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(FieldOfView))]
public class FieldOfViewEditor : Editor
{
	
	void OnSceneGUI()
	{
		FieldOfView fow = (FieldOfView)target;

		Handles.color = Color.white;
		Handles.DrawWireArc(fow.transform.position, Vector3.forward, Vector3.up, 360, fow.viewRadius);      // draw a circle

		Vector3 viewAngleA = fow.DirFromAngle(-fow.viewAngle, false);		// get angle direction
		Vector3 viewAngleB = fow.DirFromAngle(fow.viewAngle, false);

		Handles.DrawLine(fow.transform.position, fow.transform.position + viewAngleA * fow.viewRadius);		// draw a line toward angle direction till the viewRadius
		Handles.DrawLine(fow.transform.position, fow.transform.position + viewAngleB * fow.viewRadius);

		Handles.color = Color.red;
		foreach (Transform visibleTarget in fow.visibleTargets)	// Go through all the visible target 
		{
			Handles.DrawLine(fow.transform.position, visibleTarget.position);	// Draw a line toward the target
		}
	}
	
}

