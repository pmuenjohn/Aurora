using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof (CharacterController), typeof(PlayerMovement))]
public class TimeTraveller : MonoBehaviour
{
    public bool timeTravelledThisFrame = false;
    public bool inDimension2 = false;
    public float delay = 0.1f;
    public bool isTravelling = false;

    CharacterController controller;
    TransitionShift transition;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        transition = GetComponent<TransitionShift>();
    }

    void Update()
    {
        if(timeTravelledThisFrame)
            Debug.Log("TimeTravelled!");
        timeTravelledThisFrame = false;
        if(Input.GetButtonDown("TimeTravel") && !isTravelling)
        {
            StartCoroutine("TimeTravel");
        }
    }

    private IEnumerator TimeTravel()
    {
        transition.PlayTransition();
        isTravelling = true;
        yield return new WaitForSeconds(delay);
        timeTravelledThisFrame = true;
        controller.enabled = false;
        transform.position += (inDimension2 ? -100 : 100) * Vector3.up;
        controller.enabled = true;
        inDimension2 = !inDimension2;
        isTravelling = false;
    }
}
