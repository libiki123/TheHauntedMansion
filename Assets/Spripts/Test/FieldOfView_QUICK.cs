using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldOfView_QUICK : MonoBehaviour
{
    /*
       ////// QUICK WAY TO SET UP FIELD OF VIEW ///////
       TO USE: Update position & dir in the PlayerController (CheckInput())
        
        fov.SetAimDirection(mousePos);
        fov.SetOrigin(transform.position);
    */
    [SerializeField] private LayerMask layerMask;

    private Mesh mesh;
    public float fov = 90f;
    public float viewDistance = 5f;
    private Vector3 origin;
    private float startingAngle;

    private void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        origin = Vector3.zero;
    }

    private void LateUpdate()
    {
        SetupMesh();
    }

    private void SetupMesh()
    {
        int rayCount = 50;
        float angle = startingAngle;
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
            RaycastHit2D raycastHit2D = Physics2D.Raycast(origin, GetVectorFromAngle(angle), viewDistance, layerMask);  // check each raycast for collisions
            if (raycastHit2D.collider == null)
            {
                // No hit
                vertex = origin + GetVectorFromAngle(angle) * viewDistance;
            }
            else
            {
                // Hit object
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
            angle -= angleIncrease;
        }

        // Save mesh
        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.triangles = triangles;
        mesh.bounds = new Bounds(origin, Vector3.one * 1000f);
    }

    private Vector3 GetVectorFromAngle(float angle)     // Helper func
    {
        float angleRad = angle * (Mathf.PI / 180f);
        return new Vector3(Mathf.Cos(angleRad), Mathf.Sin(angleRad));
    }

    private float GetAngleFromVectorFloat(Vector3 dir)      // Helper func
    {
        dir = dir.normalized;
        float n = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        if (n < 0)
            n += 360;

        return n;
    }

    public void SetOrigin(Vector3 origin)       // change FOV start point
    {
        this.origin = origin;
    }

    public void SetAimDirection(Vector3 aimDirection)       // change FOV direction
    {
        startingAngle = GetAngleFromVectorFloat(aimDirection) + fov / 2f;
    }

    public void SetFoV(float fov)       // change FOV width
    {
        this.fov = fov;
    }

    public void SetViewDistance(float viewDistance)     // change FOV view distance
    {
        this.viewDistance = viewDistance;
    }
}
