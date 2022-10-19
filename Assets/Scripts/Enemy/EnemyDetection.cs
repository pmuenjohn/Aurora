using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EnemyDetection : MonoBehaviour
{
   
    public Transform target;
    public bool findObjectWithPlayerAimpointTag = true;
    public Vector3 lastKnownTargetLocation;

    public Transform detectionOrigin;
    // public SphereCollider peripheralCollider;
    public float visionConeAngle;
    public float detectionRange;
    public float hearingDistance = 50 ;
    public float detectionTime;
    public float currentAngleToTarget;

    private bool heardShot;

    void Start()
    {
        if(!target && findObjectWithPlayerAimpointTag)
        {
            target = GameObject.FindGameObjectWithTag("PlayerAimpoint").transform;
        }
    }
    
    public bool TargetIsDetected()
    {
        // Add vision cone detection, update lastknowntargetlocation
        Vector3 targetDir = target.position - detectionOrigin.position;
        currentAngleToTarget = Vector3.Angle(targetDir, detectionOrigin.forward);

        if(Vector3.Distance(target.position, detectionOrigin.position) <= detectionRange){
            if(currentAngleToTarget < visionConeAngle)
            {
                lastKnownTargetLocation = target.position;
                return true;
            }
        }
        return false;
    }

    public bool HeardShotInRange()
    {
        if(heardShot && Vector3.Distance(target.position, detectionOrigin.position) <= hearingDistance){
            heardShot = false;
            return true;
        }
        return false;
    }

    public void PlayerHasShot()
    {
        lastKnownTargetLocation = target.position;
        heardShot = true;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta;
        Quaternion upRayRotation = Quaternion.AngleAxis(-visionConeAngle / 2, detectionOrigin.right);
        Quaternion downRayRotation = Quaternion.AngleAxis(visionConeAngle / 2, detectionOrigin.right);
        Quaternion rightRayRotation = Quaternion.AngleAxis(-visionConeAngle / 2, detectionOrigin.up);
        Quaternion leftRayRotation = Quaternion.AngleAxis(visionConeAngle / 2, detectionOrigin.up);

        Vector3 upRayDirection = upRayRotation * transform.forward * detectionRange;
        Vector3 downRayDirection = downRayRotation * transform.forward * detectionRange;
        Vector3 rightRayDirection = rightRayRotation * transform.forward * detectionRange;
        Vector3 leftRayDirection = leftRayRotation * transform.forward * detectionRange;

        Gizmos.DrawRay(detectionOrigin.position, upRayDirection);
        Gizmos.DrawRay(detectionOrigin.position, downRayDirection);
        Gizmos.DrawRay(detectionOrigin.position, rightRayDirection);
        Gizmos.DrawRay(detectionOrigin.position, leftRayDirection);

        Vector3 detectConeCenter = detectionOrigin.position + 
        (detectionRange * Mathf.Cos((visionConeAngle / 2) * Mathf.Deg2Rad))
        * detectionOrigin.forward;
        float detectConeRadius = (detectionRange * Mathf.Sin((visionConeAngle / 2) * Mathf.Deg2Rad));
        DrawCircleGizmo(detectConeCenter, detectConeRadius, Color.magenta, 36);
    }


    void DrawCircleGizmo(Vector3 center,float radius, Color color, int segments)
    {
        Gizmos.color = color;
        const float TWO_PI = Mathf.PI * 2;
        float step = TWO_PI / (float)segments;
        float theta = 0;
        float x = radius * Mathf.Cos(theta);
        float y = radius * Mathf.Sin(theta);
        Vector3 pos = center + new Vector3(0, x, y);
        Vector3 newPos;
        Vector3 lastPos = pos;
        for (theta = step; theta < TWO_PI; theta += step)
        {
            x = radius * Mathf.Cos(theta);
            y = radius * Mathf.Sin(theta);
            newPos = center + new Vector3(0, x, y);
            Gizmos.DrawLine(pos, newPos);
            pos = newPos;
        }
        Gizmos.DrawLine(pos, lastPos);

    }
}
