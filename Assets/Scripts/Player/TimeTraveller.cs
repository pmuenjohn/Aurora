using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof (CharacterController), typeof(PlayerMovement))]
public class TimeTraveller : MonoBehaviour
{
    public bool timeTravelledThisFrame = false;
    public bool inDimension2 = false;

    CharacterController controller;

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        if(Input.GetButtonDown("TimeTravel"))
        {
            timeTravelledThisFrame = true;
        }
        if(timeTravelledThisFrame)
        {
            controller.enabled = false;
            transform.position += (inDimension2 ? -100 : 100) * Vector3.up;
            controller.enabled = true;
            inDimension2 = !inDimension2;
            timeTravelledThisFrame = false;
        }
    }
}
