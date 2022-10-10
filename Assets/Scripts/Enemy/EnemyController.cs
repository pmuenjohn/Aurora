using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public EnemyDetection detection;
    public DroneZMovement movement;
    public EnemyWeaponController weapon;
    public bool targetIsInMemory;

    public enum AIState {
        Wandering,
        Combat,
        Retargeting
    }

    public float retargetingTime = 2.0f;
    public float retargetingTurnSpeed;
    private float timeLastSeenTarget = Mathf.NegativeInfinity;

    public AIState currentState;



    void Update()
    {
        UpdateState();
        RunCurrentState();
    }

    void UpdateState()
    {
        switch(currentState)
        {
            case AIState.Wandering:
                if(detection.HeardShotInRange())
                {
                    targetIsInMemory = true;
                    timeLastSeenTarget = Time.time;
                    currentState = AIState.Retargeting;
                }
                if(detection.TargetIsDetected())
                {
                    targetIsInMemory = true;
                    timeLastSeenTarget = Time.time;
                    currentState = AIState.Combat;
                }
            break;
            case AIState.Combat:
                if(!detection.TargetIsDetected())
                {
                    timeLastSeenTarget = Time.time;
                    currentState = AIState.Retargeting;
                }
            break;
            case AIState.Retargeting:
                if(Time.time - timeLastSeenTarget > retargetingTime)
                {
                    targetIsInMemory = false;
                }
                if(detection.TargetIsDetected())
                {
                    currentState = AIState.Combat;
                }
                if(!targetIsInMemory)
                {
                    currentState = AIState.Wandering;
                }
            break;
        }
    }

    void RunCurrentState()
    {
        switch(currentState)
        {
            case AIState.Wandering:
                OrientTowards(movement.nextWaypoint, movement.turnSpeedMultiplier);
            break;
            case AIState.Combat:
                OrientTowards(detection.lastKnownTargetLocation, weapon.turnSpeedMultiplier);
                TryAttack(detection.lastKnownTargetLocation);
            break;
            case AIState.Retargeting:
                Debug.Log("retargeihn");
                OrientTowards(detection.lastKnownTargetLocation, retargetingTurnSpeed);
            break;
        }
    }

    void OrientTowards(Vector3 targetLocation, float turnSpeedMultiplier)
    {
        Vector3 targetDirection  = (targetLocation - transform.position).normalized;
        if(targetDirection.sqrMagnitude != 0f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * turnSpeedMultiplier);
        }
    }

    bool TryAttack(Vector3 targetPosition)
    {
        return weapon.FireWeapon();
    }
    
}
