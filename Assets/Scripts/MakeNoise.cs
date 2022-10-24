using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MakeNoise : MonoBehaviour
{
    public EnemyDetection[] detections;
    public UnityAction onHeardShot;

    void Start()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        detections = new EnemyDetection[enemies.Length];
        for(int i = 0; i < enemies.Length; i++)
        {
            detections[i] = enemies[i].GetComponent<EnemyDetection>();
            onHeardShot += detections[i].PlayerHasShot;
        }
    }

    public void PlayerHasShot()
    {
        if(detections.Length > 0)
        {
        onHeardShot.Invoke();
        }
    }
}
