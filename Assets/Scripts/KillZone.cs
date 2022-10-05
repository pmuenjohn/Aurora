using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillZone : MonoBehaviour
{
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.layer == (int)Layer.Player)
        {
            PlayerStatus player = other.gameObject.GetComponent<PlayerStatus>();
            if (player != null)
            {
                player.TakeDamage(999999f);
                Debug.Log("KILL ZONE");
            }
        }
    }
}
