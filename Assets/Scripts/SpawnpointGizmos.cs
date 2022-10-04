using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnpointGizmos : MonoBehaviour
{

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(transform.position, 0.5f);
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + transform.forward * 2f);
    }
}
