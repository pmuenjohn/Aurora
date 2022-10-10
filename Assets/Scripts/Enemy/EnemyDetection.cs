using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDetection : MonoBehaviour
{
   
    public Transform target;
    public Vector3 lastKnownTargetLocation;

    public Transform detectionOrigin;
    public SphereCollider peripheralCollider;
    public float visionConeAngle;
    public float detectionRange;
    public float detectionTime;
    
    public bool TargetIsDetected()
    {
        // Add vision cone detection, update lastknowntargetlocation
        if(Vector3.Distance(target.position, detectionOrigin.position) <= detectionRange){
            lastKnownTargetLocation = target.position;
            return true;
        }
        else
        {
            return false;
        }
        
    }
    
}
