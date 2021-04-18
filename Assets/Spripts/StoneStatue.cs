using UnityEngine;

public class StoneStatue : EnemyController
{
    Vector2 startPos;
    Vector3 startRotation;

    protected override void Start()
    {
        base.Start();
        startPos = transform.position;
        startRotation = transform.eulerAngles;
    }

    private void Patrolling()
    {
        bool seePlayer = player != null && !isOutOfZone && (fov.visibleTargets.Count > 0 || Physics2D.OverlapCircle(transform.position, aggroRange, fov.targetMask));

;       if (seePlayer && !isStuned)     // Check if Player is in aggroRange
        {
            state = State.Chasing;
            return;
        }
        
        agent.speed = speed * 0.5f; ;
        agent.destination = startPos;
        
        if((Vector2)transform.position == startPos)
		{
            transform.eulerAngles = startRotation;
        }
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
