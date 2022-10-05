using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public Transform spawnpoint;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == (int)Layer.Player)
        {
            PlayerStatus.hasCurrentCheckpoint = true;
            PlayerStatus.currentCheckpointPos = transform.position;
            PlayerStatus.currentCheckpointRot = transform.rotation;
        }
    }
}
