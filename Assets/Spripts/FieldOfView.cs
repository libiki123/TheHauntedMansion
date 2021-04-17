
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldOfView : MonoBehaviour
{
	[HideInInspector]
	public List<Transform> visibleTargets = new List<Transform>();

	[Space]
	[Header("Field of View")]
	public float viewRadius = 7f;
	[Range(0, 180)]
	public float viewAngle;

	[Space]
	[Header("Layer Masks")]
	public LayerMask targetMask;
	public LayerMask obstacleMask;

	[Space]
	[Header("Mesh")]
	public Vector3 meshStartpoint = new Vector3(0, 0, 0);
	public int rayCount;
	public int edgeResolveRayCount;		// amount of extra raycast to handle edge
	public float edgeDstThreshold;			// handle multiple edge hit

	public MeshFilter viewMeshFilter;

	Mesh viewMesh;
	float originViewRadius;
	float originViewAngle;


	void Start()
	{
		viewMesh = new Mesh();
		viewMesh.name = "View Mesh";
		viewMeshFilter.mesh = viewMesh;

		originViewRadius = viewRadius;
		originViewAngle = viewAngle;

		StartCoroutine("FindTargetsWithDelay", .1f);
	}


	IEnumerator FindTargetsWithDelay(float delay)       // Repeat FindVisibleTargets() with a delay
	{
		while (true)
		{
			yield return new WaitForSeconds(delay);
			FindVisibleTargets();
		}
	}

	void LateUpdate()
	{
		DrawFieldOfView();
	}

	void FindVisibleTargets()	
	{
		visibleTargets.Clear();
		Collider2D[] targetsInViewRadius = Physics2D.OverlapCircleAll(transform.position, viewRadius, targetMask);		// Find all targets within viewRadius

		for (int i = 0; i < targetsInViewRadius.Length; i++)
		{
			Transform target = targetsInViewRadius[i].transform;
			Vector3 dirToTarget = (transform.position - target.position).normalized;
			if (Vector3.Angle(transform.up, dirToTarget) < viewAngle)					// Find all target within viewAngle
			{
				float dstToTarget = Vector3.Distance(transform.position, target.position);
				if (!Physics.Raycast(transform.position, dirToTarget, dstToTarget, obstacleMask))		// Find all targets not blocked by obstacleMask
				{
					visibleTargets.Add(target);
				}
			}
		}
	}

	void DrawFieldOfView()		// Generate raycast and mesh
	{
		float stepAngleSize = viewAngle*2 / rayCount;
		List<Vector3> viewPoints = new List<Vector3>();
		ViewCastInfo oldViewCast = new ViewCastInfo();

		for (int i = 0; i <= rayCount; i++)
		{
			float angle = transform.eulerAngles.z -90f - viewAngle + stepAngleSize * i;
			ViewCastInfo newViewCast = ViewCast(angle);

				if (i > 0)	// skip origin
				{
					bool edgeDstThresholdExceeded = Mathf.Abs(oldViewCast.dst - newViewCast.dst) > edgeDstThreshold;
					if (oldViewCast.hit != newViewCast.hit || (oldViewCast.hit && newViewCast.hit && edgeDstThresholdExceeded))		// handle edge raycast
					{
						EdgeInfo edge = FindEdge(oldViewCast, newViewCast);
						if (edge.pointA != Vector3.zero)
						{
							viewPoints.Add(edge.pointA);
						}
						if (edge.pointB != Vector3.zero)
						{
							viewPoints.Add(edge.pointB);
						}
					}

				}


			viewPoints.Add(newViewCast.point);
			oldViewCast = newViewCast;
		}

		int vertexCount = viewPoints.Count + 1;
		Vector3[] vertices = new Vector3[vertexCount];
		int[] triangles = new int[(vertexCount - 2) * 3];

		vertices[0] = Vector3.zero + meshStartpoint;	// STARTING POINT OF THE MESH
		for (int i = 0; i < vertexCount - 1; i++)
		{
			vertices[i + 1] = transform.InverseTransformPoint(viewPoints[i]);

			if (i < vertexCount - 2)
			{
				triangles[i * 3] = 0;
				triangles[i * 3 + 1] = i + 1;
				triangles[i * 3 + 2] = i + 2;
			}
		}

		viewMesh.Clear();

		viewMesh.vertices = vertices;
		viewMesh.triangles = triangles;
		viewMesh.RecalculateNormals();
	}


	EdgeInfo FindEdge(ViewCastInfo minViewCast, ViewCastInfo maxViewCast)	// raycast more at the edge of an object to make it smoother
	{																		// find in - out pair (min - max), keep casting until out - out
		float minAngle = minViewCast.angle;
		float maxAngle = maxViewCast.angle;
		Vector3 minPoint = Vector3.zero;
		Vector3 maxPoint = Vector3.zero;

		for (int i = 0; i < edgeResolveRayCount; i++)
		{
			float angle = (minAngle + maxAngle) / 2;
			ViewCastInfo newViewCast = ViewCast(angle);     // put new raycast between min & max


			bool edgeDstThresholdExceeded = Mathf.Abs(minViewCast.dst - newViewCast.dst) > edgeDstThreshold;		
			if (newViewCast.hit == minViewCast.hit && !edgeDstThresholdExceeded)
			{
				minAngle = angle;
				minPoint = newViewCast.point;
			}
			else
			{
				maxAngle = angle;
				maxPoint = newViewCast.point;
			}
		}

		return new EdgeInfo(minPoint, maxPoint);
	}


	ViewCastInfo ViewCast(float globalAngle)	// Checking Raycast base on angle
	{
		Vector3 dir = DirFromAngle(globalAngle, true);
		RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, viewRadius, obstacleMask);

		if (hit.collider != null)
		{
			//Debug.DrawLine(transform.position, transform.position + dir * hit.distance, Color.red);
			return new ViewCastInfo(true, hit.point, hit.distance, globalAngle);		// HIT
		}
		else
		{
			//Debug.DrawLine(transform.position, transform.position + dir * viewRadius, Color.red);
			return new ViewCastInfo(false, transform.position + dir * viewRadius, viewRadius, globalAngle);     // NOT HIT
		}
	}

	public Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal)	// Helper func
	{
		if (!angleIsGlobal)		//If not global, angle will be relative to player
		{
			angleInDegrees += transform.eulerAngles.z - 90f;
		}
		return new Vector3(Mathf.Cos(angleInDegrees * Mathf.Deg2Rad), Mathf.Sin(angleInDegrees * Mathf.Deg2Rad));
	}

	public void ResetFov()
	{
		viewRadius = originViewRadius;
		viewAngle = originViewAngle;
	}

	public struct ViewCastInfo
	{
		public bool hit;
		public Vector3 point;
		public float dst;
		public float angle;

		public ViewCastInfo(bool _hit, Vector3 _point, float _dst, float _angle)
		{
			hit = _hit;
			point = _point;
			dst = _dst;
			angle = _angle;
		}
	}

	public struct EdgeInfo
	{
		public Vector3 pointA;
		public Vector3 pointB;

		public EdgeInfo(Vector3 _pointA, Vector3 _pointB)
		{
			pointA = _pointA;
			pointB = _pointB;
		}
	}

}
