using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ghost : EnemyController
{
    int waypointIndex;                  // the current waypoint index in the waypoints array
    public Transform[] waypoints;       // collection of waypoints which define a patrol area

   

    [Header("Patrol")]
    public float patrolTime = 10;       // time in seconds to wait before seeking a new patrol destination

	protected override void Start()
	{
		base.Start();

        if(waypoints.Length <= 0)
            waypoints = zone.waypointsInZone;


        if (waypoints.Length > 0)       // If more than 1 waypoint
        {
            InvokeRepeating("Patrol", Random.Range(0, patrolTime), patrolTime);     // Find the next waypoint
        }
    }

    private void Patrol()
    {
        waypointIndex = waypointIndex == waypoints.Length - 1 ? 0 : waypointIndex + 1;      // If reached the waypoint find the next waypoint
    }

	private void Patrolling()
	{
        bool seePlayer = player != null && !isOutOfZone && (fov.visibleTargets.Count > 0 || Physics2D.OverlapCircle(transform.position, aggroRange, fov.targetMask));
        if (seePlayer)     // Check if Player is in aggroRange
        {
            state = State.Chasing;
            return;
        }

        agent.destination = waypoints[waypointIndex].position;      // Set the destination base on the waypoint
        agent.speed = speed * 0.5f;                                    // Set speed to walk (agentSpeed / 2)
    }



	protected override void CheckState()
	{
		base.CheckState();

        switch (state)
        {
            default:
            case State.Patrolling:
                Invoke("Patrolling", 0.5f);       // Call Tick() repeatedly
                break;
            case State.Chasing:
                Chasing();
                break;
        }
    }

}
