using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class EnemyController : MonoBehaviour
{
    protected enum State { Patrolling, Chasing}

    protected Transform player;
    protected Rigidbody2D rb;
    protected FieldOfView fov;
    protected NavMeshAgent agent;
    protected LineRenderer line;

    protected State state;

    protected float speed;                // current agent speed and NavMeshAgent component speed
    protected bool isOutOfZone = false;
    protected bool isStuned = false;

    float stunTimer;

    [Header("Zone")]
    public Zone zone;

    [Space]
    [Header("Movements")]
    public float aggroRange;
    public float turnSpeed = 5f;

    [Space]
    [Header("Timers")]
    public float stunTimerSet = 3f;

    protected virtual void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        fov = GetComponent<FieldOfView>();
        rb = GetComponent<Rigidbody2D>();
        agent = GetComponent<NavMeshAgent>();
        line = GetComponent<LineRenderer>();

        agent.updateRotation = false;
        agent.updateUpAxis = false;
        speed = agent.speed;
        stunTimer = stunTimerSet;

        state = State.Patrolling;
    }

    void Update()
    {
        CheckState();
        DrawPath();
    }

	private void LateUpdate()
	{
		FaceMovingPos();
	}

	private void FaceMovingPos()
	{
		if (agent.velocity == Vector3.zero)
		{
			return;
		}

		float angle = Mathf.Atan2(agent.velocity.y, agent.velocity.x) * Mathf.Rad2Deg - 90f;
		Quaternion rotation = Quaternion.Euler(0, 0, angle);
		transform.rotation = Quaternion.Slerp(transform.rotation, rotation, turnSpeed * Time.deltaTime);
	}

	public void Stun()
    {
        isStuned = true;
    }

    private bool IsOutOfZone()
    {
        float distance = Vector2.Distance(zone.transform.position, transform.position);

        if (distance > zone.zoneRadius)
            return true;

        return false;
    }

    protected virtual void CheckState()
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

    protected void Chasing()
    {
        if (isStuned)        // Stop all movement if stunned
        {
            agent.speed = 0;
            agent.destination = player.position;
            return;
        }

        if (Vector2.Distance(transform.position, player.position) < fov.viewRadius && !isOutOfZone)
        {
            agent.speed = speed;                    // Set the destination to the Player
            agent.destination = player.position;    // Set speed to run
        }
        else
        {
            state = State.Patrolling;
        }
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
