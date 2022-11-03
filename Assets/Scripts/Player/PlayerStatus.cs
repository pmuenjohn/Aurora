using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameCommon;

public class PlayerStatus : MonoBehaviour
{
    public float maxHP = 10;
    public float hp;
    public static bool hasCurrentCheckpoint = false;
    public static Vector3 currentCheckpointPos;
    public static Quaternion currentCheckpointRot;

    [Header("Player canvas reference - not nullable")]
    public PlayerCanvas playerCanvas;

    private void Start()
    {
        hp = maxHP;

        //only set player pos to checkpoint if has checkpoint. If not, default player pos is kept untouched
        if (hasCurrentCheckpoint)
        {
            CharacterController ctrl = gameObject.GetComponent<CharacterController>();
            ctrl.enabled = false;
            transform.rotation = currentCheckpointRot;
            transform.position = currentCheckpointPos;
            Debug.Log("player rot set to " + currentCheckpointRot);
            ctrl.enabled = true;
        }
    }

    public void TakeDamage(float amount)
    {
        hp -= amount;
        if (hp <= 0)
        {
            playerCanvas.DisplayDeathScreen();
        }
    }

    public void ClearCheckpoint()
    {
        hasCurrentCheckpoint = false;
    }
}
