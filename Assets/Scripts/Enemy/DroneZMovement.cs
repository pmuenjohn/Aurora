using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class DroneZMovement : Enemy
{
    [Header("Changable Movement Values")]
    public float minSpeed = 2f;
    public float maxSpeed = 2f;
    public bool hasDynamicMovementRange = false;
    public float minRange = 2f;
    public float maxRange = 5f;
    public float movementCooldown = 1.5f; //in seconds
    public Vector3 nextWaypoint;
    public Ease movementEase = Ease.Linear;
    private float nextMoveTime;

    [Header("Movement Status")]
    public bool isMoving;
    private Vector3 movementRangeCenter;

    private void OnEnable()
    {
        nextMoveTime = Time.time;
        movementRangeCenter = transform.position;
    }

    private void Update()
    {
        if (!isMoving && Time.time >= nextMoveTime)
        {
            Move();
        }
    }

    private void Move()
    {
        isMoving = true;

        //get new position
        if (hasDynamicMovementRange)
            movementRangeCenter = transform.position;
        nextWaypoint = Random.onUnitSphere * Random.Range(minRange, maxRange) + movementRangeCenter;

        //move
        float moveDistance = Vector3.Distance(nextWaypoint, transform.position);
        transform.DOMove(nextWaypoint, 1 / Random.Range(minSpeed, maxSpeed) * moveDistance)
            .SetEase(movementEase)
            .OnComplete(GenerateNextMove);
    }

    private void GenerateNextMove()
    {
        isMoving = false;
        nextMoveTime = Time.time + movementCooldown;
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
