using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * 1. Player must be in sightrange
 * 2. Player must be in danger zone
 */

public class PirateAIController : BoatController
{
    public Transform playerTarget;
    public DangerZone dangerZone;
    public Transform[] patrolPointArray;
    public Queue<Transform> patrolPoints;

    public float angularTrackingAccuracy = 15;

    public Transform currentPatrolPointDebug;

    public float sightRange = 20.0f;
    public bool inChase = false;

    public void Start()
    {
        patrolPoints = new Queue<Transform>();
        foreach (Transform patrolPoint in patrolPointArray)
        {
            patrolPoints.Enqueue(patrolPoint);
        }
        currentPatrolPointDebug = patrolPoints.Peek();
    }

    public void CalculateNavigation(Transform target)
    {
        Vector2 dir = (target.position - this.transform.position).normalized;

        float xdiff = Vector2.Angle(this.transform.right, dir);
        float ydiff = Vector2.Angle(this.transform.up, dir);

        if (ydiff < 90)
        {
            forward = true;

            if (xdiff > 90 + angularTrackingAccuracy)
            {
                right = false;
                left = true;
            }
            else if (xdiff < 90 - angularTrackingAccuracy)
            {
                right = true;
                left = false;
            }
            else
            {
                right = false;
                left = false;
            }
        }
        else 
        {
            forward = false;
            if (xdiff > 90)
            {
                right = false;
                left = true;
            }
            else
            {
                right = true;
                left = false;
            }
        }
    }

    public Transform GetNextPatrolPoint()
    {
        Transform nextTransform = patrolPoints.Dequeue();
        patrolPoints.Enqueue(nextTransform);
        currentPatrolPointDebug = patrolPoints.Peek();
        return patrolPoints.Peek();
    }

    private void Update() 
    {
        // Check for sightRange
        if (inChase)
        {
            CalculateNavigation(playerTarget);
        }
        else
        {
            CalculateNavigation(patrolPoints.Peek());
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "PatrolPoint")
        {
            if(collision.transform == patrolPoints.Peek())
            {
                GetNextPatrolPoint();
            }
        }
    }
}
