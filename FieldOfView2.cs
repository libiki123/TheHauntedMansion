using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldOfView2 : MonoBehaviour
{
	
    private MeshFilter meshFilter;
    private Transform transform;

    private Mesh mesh;

    public float fov = 90f;
    public int rayCount = 50;
    public float viewDistance = 5f;

    void Start()
    {
        meshFilter = GetComponent<MeshFilter>();
        transform = GetComponent<Transform>();


        mesh = new Mesh();
        meshFilter.mesh = mesh;
        
    }

    void Update()
    {
		SetupMesh();
	}

    private void SetupMesh()
	{

		Vector3 origin = Vector3.zero;
		float angle = 0f;
		float angleIncrease = fov / rayCount;

		Vector3[] vertices = new Vector3[rayCount + 1 + 1];
		Vector2[] uv = new Vector2[vertices.Length];
		int[] triangles = new int[rayCount * 3];

		vertices[0] = origin;

		int vertexIndex = 1;
		int triangleIndex = 0;
		for (int i = 0; i <= rayCount; i++)
		{
			Vector3 vertex;
			RaycastHit2D raycastHit2D = Physics2D.Raycast(origin, GetVectorFromAngle(angle), viewDistance);

			if (raycastHit2D.collider == null)       // Check it raycast collide with any objects
			{        // no hit                        
				vertex = origin + GetVectorFromAngle(angle) * viewDistance;
			}
			else     // Hit
			{
				vertex = raycastHit2D.point;
			}

			vertices[vertexIndex] = vertex;

			if (i > 0)
			{
				triangles[triangleIndex + 0] = 0;
				triangles[triangleIndex + 1] = vertexIndex - 1;
				triangles[triangleIndex + 2] = vertexIndex;

				triangleIndex += 3;
			}

			vertexIndex++;
			angle -= angleIncrease;         // increase the angle clock wide (unity do the opposite)

		}

		mesh.vertices = vertices;
		mesh.uv = uv;
		mesh.triangles = triangles;
    }
	

    public Vector3 GetVectorFromAngle(float angle)
	{
        float angleRad = angle * (Mathf.PI / 180f);
        return new Vector3(Mathf.Cos(angleRad), Mathf.Sin(angleRad));
    }
	





	

	/*
	public float viewRadius;
	[Range(0, 180)]
	public float viewAngle;

	public LayerMask targetMask;
	public LayerMask obstacleMask;

	[HideInInspector]
	public List<Transform> visibleTargets = new List<Transform>();

	public float meshResolution;
	public int edgeResolveIterations;
	public float edgeDstThreshold;

	public MeshFilter viewMeshFilter;
	Mesh viewMesh;

	void Start()
	{
		viewMesh = new Mesh();
		viewMesh.name = "View Mesh";
		viewMeshFilter.mesh = viewMesh;

		StartCoroutine("FindTargetsWithDelay", .2f);
	}

	void LateUpdate()
	{
		DrawFieldOfView();
	}

	IEnumerator FindTargetsWithDelay(float delay)
	{
		while (true)
		{
			yield return new WaitForSeconds(delay);
			FindVisibleTargets();
		}
	}


	void FindVisibleTargets()		
	{
		visibleTargets.Clear();
		Collider2D[] targetsInViewRadius = Physics2D.OverlapCircleAll(transform.position, viewRadius, targetMask);
		for (int i = 0; i < targetsInViewRadius.Length; i++)    // Find all target within the radius
		{
			Transform target = targetsInViewRadius[i].transform;
			Vector2 dirToTarget = (transform.position - target.position).normalized;

			if (Vector2.Angle(transform.up, dirToTarget) < viewAngle)		// Finall all target within view angle
			{
				float dstToTarget = Vector2.Distance(transform.position, target.position);
				if (!Physics2D.Raycast(transform.up, dirToTarget, dstToTarget, obstacleMask))   // Raycast and check if target being blocked by obstacleMask
				{
					visibleTargets.Add(target);
				}
			}
		}
	}

	void DrawFieldOfView()
	{
		int stepCount = Mathf.RoundToInt(viewAngle * meshResolution);
		float stepAngleSize = viewAngle / stepCount;
		List<Vector2> viewPoints = new List<Vector2>();
		ViewCastInfo oldViewCast = new ViewCastInfo();
		for (int i = 0; i <= stepCount; i++)
		{
			float angle = transform.eulerAngles.z - 90f - viewAngle + stepAngleSize * i * 2;

			Debug.DrawLine(transform.position, transform.position + DirFromAngle(angle, true) * viewRadius, Color.red);
			
			ViewCastInfo newViewCast = ViewCast(angle);

			//if (i > 0)
			//{
			//	bool edgeDstThresholdExceeded = Mathf.Abs(oldViewCast.dst - newViewCast.dst) > edgeDstThreshold;
			//	if (oldViewCast.hit != newViewCast.hit || (oldViewCast.hit && newViewCast.hit && edgeDstThresholdExceeded))
			//	{
			//		EdgeInfo edge = FindEdge(oldViewCast, newViewCast);
			//		if (edge.pointA != Vector3.zero)
			//		{
			//			viewPoints.Add(edge.pointA);
			//		}
			//		if (edge.pointB != Vector3.zero)
			//		{
			//			viewPoints.Add(edge.pointB);
			//		}
			//	}

			//}


			viewPoints.Add(newViewCast.point);
			oldViewCast = newViewCast;
			
		}

		int vertexCount = viewPoints.Count + 1;
		Vector3[] vertices = new Vector3[vertexCount];
		int[] triangles = new int[(vertexCount - 2) * 3];

		vertices[0] = Vector3.zero;
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

	
	EdgeInfo FindEdge(ViewCastInfo minViewCast, ViewCastInfo maxViewCast)
	{
		float minAngle = minViewCast.angle;
		float maxAngle = maxViewCast.angle;
		Vector3 minPoint = Vector3.zero;
		Vector3 maxPoint = Vector3.zero;

		for (int i = 0; i < edgeResolveIterations; i++)
		{
			float angle = (minAngle + maxAngle) / 2;
			ViewCastInfo newViewCast = ViewCast(angle);

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
	

	ViewCastInfo ViewCast(float globalAngle)
	{
		Vector3 dir = DirFromAngle(globalAngle, true);
		RaycastHit hit;

		RaycastHit2D raycastHit2D = Physics2D.Raycast(transform.up, dir, viewRadius, obstacleMask);

		if (raycastHit2D.collider == null)       // Check it raycast collide with any objects
		{        // no hit                        
			vertex = origin + GetVectorFromAngle(angle) * viewDistance;
		}
		else     // Hit
		{
			Debug.Log("HIT");
			vertex = raycastHit2D.point;
		}

		//if (Physics2D.Raycast(transform.up, dir, viewRadius, obstacleMask))
		//{
		//	Debug.Log("HIT");
		//	return new ViewCastInfo(true, , globalAngle);
		//}
		//else
		//{
		//	return new ViewCastInfo(false, transform.position + dir * viewRadius, viewRadius, globalAngle);
		//}
	}

	public Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal)
	{
		if (!angleIsGlobal)		// if not global move it follow the transform
		{
			angleInDegrees += transform.eulerAngles.z - 90f;
		}
		return new Vector3(Mathf.Cos(angleInDegrees * Mathf.Deg2Rad), Mathf.Sin(angleInDegrees * Mathf.Deg2Rad));
	}


	public struct ViewCastInfo
	{
		public bool hit;
		public Vector3 point;
		public float dst;
		public float angle;

		public ViewCastInfo(bool _hit, Vector2 _point, float _dst, float _angle)
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
	*/
}
