using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class DroneZMovement : Enemy
{
    [Header("Changable Movement Values")]
    public float minSpeed = 5f;
    public float maxSpeed = 5f;
    public float acceleration = 0.1f;
    public bool hasDynamicMovementRange = false;
    public float minRange = 2f;
    public float maxRange = 5f;
    public float minMovementCooldown = 0.5f; //in seconds
    public float maxMovementCooldown = 1.5f; //in seconds
    public Ease movementEase = Ease.Linear;
    private float nextMoveTime;
    public float maxMovementDuration = 1f;
    private float endOfMovementTime;
    public float turnSpeedMultiplier;

    [Header("Movement Status")]
    public bool isMoving;
    public Vector3 nextWaypoint;
    private Vector3 movementRangeCenter;
    private Rigidbody rb;

    private void OnEnable()
    {
        nextMoveTime = Time.time;
        movementRangeCenter = transform.position;
        rb = GetComponent<Rigidbody>();
        GenerateNextMove();
    }

    private void Update()
    {
        if (Time.time >= nextMoveTime)
        {
            Move();
        }
    }

    private void Move()
    {
        isMoving = true;

        //move
        //float moveDistance = Vector3.Distance(nextWaypoint, transform.position);
        //transform.DOMove(nextWaypoint, 1 / Random.Range(minSpeed, maxSpeed) * moveDistance)
        //    .SetEase(movementEase)
        //    .OnComplete(GenerateNextMove);

        rb.velocity = (nextWaypoint - transform.position).normalized * Random.Range(minSpeed, maxSpeed);

        //stop movement
        if (Vector3.Distance(nextWaypoint, transform.position) < 0.1f ||
            Time.time >= endOfMovementTime)
        {
            rb.velocity = Vector3.zero;
            GenerateNextMove();
        }

    }

    private void GenerateNextMove()
    {
        //get new position
        if (hasDynamicMovementRange)
            movementRangeCenter = transform.position;
        nextWaypoint = Random.onUnitSphere * Random.Range(minRange, maxRange) + movementRangeCenter;
        isMoving = false;
        nextMoveTime = Time.time + Random.Range(minMovementCooldown, maxMovementCooldown);
        endOfMovementTime = nextMoveTime + maxMovementDuration;
    }

    private void OnDestroy()
    {
        transform.DOKill();
    }

    private void OnDrawGizmosSelected()
    {
        //max moverange
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(movementRangeCenter, maxRange);

        //min moverange
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(movementRangeCenter, minRange);

        //next waypoint
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(nextWaypoint, 0.1f);
    }
}
