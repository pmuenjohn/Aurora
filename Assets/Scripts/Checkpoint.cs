using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public Transform spawnpoint;

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.layer == (int)Layer.Player)
        {
            PlayerStatus.hasCurrentCheckpoint = true;
            PlayerStatus.currentCheckpointPos = spawnpoint.position;
            PlayerStatus.currentCheckpointRot = spawnpoint.rotation;
        }
    }
}
