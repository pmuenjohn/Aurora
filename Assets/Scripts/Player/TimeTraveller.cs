using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof (CharacterController), typeof(PlayerMovement))]
public class TimeTraveller : MonoBehaviour
{
    public bool timeTravelledThisFrame = false;
    public bool inDimension2 = false;
    public float delay = 0.1f;

    CharacterController controller;

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        if(timeTravelledThisFrame)
            Debug.Log("TimeTravelled!");
        timeTravelledThisFrame = false;
        if(Input.GetButtonDown("TimeTravel"))
        {
            StartCoroutine("TimeTravel");
        }
    }

    private IEnumerator TimeTravel()
    {
        yield return new WaitForSeconds(delay);
        timeTravelledThisFrame = true;
        controller.enabled = false;
        transform.position += (inDimension2 ? -100 : 100) * Vector3.up;
        controller.enabled = true;
        inDimension2 = !inDimension2;
    }
}
