using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    private Transform player;
    private Rigidbody2D rb;
    private FieldOfView fov;
    private NavMeshAgent agent;
    private LineRenderer line;

    int waypointIndex;                  // the current waypoint index in the waypoints array
    private Transform[] waypoints;       // collection of waypoints which define a patrol area

    float speed;                // current agent speed and NavMeshAgent component speed
    bool isStuned = false;
    bool isOutOfZone = false;


    public float aggroRange;
    public float turnSpeed = 5f;

    public float patrolTime = 10;       // time in seconds to wait before seeking a new patrol destination
    
    public Zone zone;

    [Space]
    [Header("Timers")]
    float stunTimer;
    public float stunTimerSet = 4f;


    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        fov = GetComponent<FieldOfView>();
        rb = GetComponent<Rigidbody2D>();
        agent = GetComponent<NavMeshAgent>();
        line = GetComponent<LineRenderer>();
        

        stunTimer = stunTimerSet;
        waypoints = zone.waypointsInZone;

        agent.updateRotation = false;
        agent.updateUpAxis = false;
        speed = agent.speed;

        InvokeRepeating("Tick", 0, 0.5f);       // Call Tick() repeatedly
		if (waypoints.Length > 0)       // If more than 1 waypoint
		{
			InvokeRepeating("Patrol", Random.Range(0, patrolTime), patrolTime);     // Find the next waypoint
		}

	}

    void Update()
    {
        CheckSatus();
        DrawPath();
    }

	private void LateUpdate()
	{
        FaceMovingPos();
    }

    private void FaceMovingPos()
	{
        float angle = Mathf.Atan2(agent.velocity.y, agent.velocity.x) * Mathf.Rad2Deg + 90f;
        Quaternion rotation = Quaternion.Euler(0, 0, angle);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, turnSpeed * Time.deltaTime);
    }

	private void Patrol()
    {
        waypointIndex = waypointIndex == waypoints.Length - 1 ? 0 : waypointIndex + 1;      // If reached the waypoint find the next waypoint
    }

    private void Tick()
    {
		if(isStuned)        // Stop all movement if stunned
		{
            agent.speed = 0;
            agent.destination = Vector3.zero;
            return;                                 
		}

        if (player != null && !isOutOfZone &&
            (fov.visibleTargets.Count > 0 || Physics2D.OverlapCircle(transform.position, aggroRange, fov.targetMask)))     // Check if Player is in aggroRange
		{
			agent.speed = speed;                    // Set the destination to the Player
			agent.destination = player.position;    // Set speed to run
            return;
		}

        agent.destination = waypoints[waypointIndex].position;      // Set the destination base on the waypoint
        agent.speed = speed * 0.5f;                                    // Set speed to walk (agentSpeed / 2)
	}

    public void Stun()
	{
        isStuned = true;
    }

    private void CheckSatus()
	{
        if (isStuned)
        {
            stunTimer -= Time.deltaTime;
            if (stunTimer <= 0)
            {
                stunTimer = stunTimerSet;
                isStuned = false;
            }
        }

        isOutOfZone = IsOutOfZone();
    }

    private bool IsOutOfZone()
	{
        //float distance = isOutOfZone? 0 : Vector2.Distance(zone.transform.position, transform.position);
        float distance = Vector2.Distance(zone.transform.position, transform.position);

        if (distance > zone.zoneRadius)
            return true;

        return false;
	}

    void DrawPath()
    {
        line.SetPosition(0, transform.position);

        if (agent.path.corners.Length < 2) //if the path has 1 or no corners, there is no need
            return;

        line.SetVertexCount(agent.path.corners.Length); //set the array of positions to the amount of corners

        for (var i = 1; i < agent.path.corners.Length; i++)
        {
            line.SetPosition(i, agent.path.corners[i]); //go through each corner and set that to the line renderer's position
        }
    }

	private void OnCollisionEnter2D(Collision2D collision)
	{
        // check if collide with player
        if(collision.gameObject.tag == "Player")
		{
            collision.gameObject.GetComponent<PlayerController>().TakeDamage(transform);
		}
	}


	private void OnDrawGizmos()
	{
        Gizmos.DrawWireSphere(transform.position, aggroRange);
	}
}
