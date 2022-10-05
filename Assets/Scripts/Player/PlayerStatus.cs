using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameCommon;

public class PlayerStatus : MonoBehaviour
{
    public float maxHP = 10;
    public float hp;
    public Checkpoint startCheckpoint;
    [HideInInspector]
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
            transform.position = currentCheckpointPos;
            transform.rotation = currentCheckpointRot;
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
}
