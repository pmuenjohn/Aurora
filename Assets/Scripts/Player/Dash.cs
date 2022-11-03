using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Dash : MonoBehaviour
{
    public PlayerMovement movement;
    public PlayerInput input;
    public CharacterController controller;

    public float dashSpeed = 15;
    public float dashTime = 0.25f;
    public float dashCoolTime = 1;

    public bool isDashing = false;
    public bool canDash = true;

    public Vector3 lastHorizontalVelocity;

    float startTime;
    public float startCooldownTime;

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.E) && !isDashing && canDash)
        {
            StartCoroutine(BeginDash());
        }
    }

    IEnumerator BeginDash()
    {
        isDashing = true;
        controller.velocity.Set(0,0,0);
        // movement.canInputMovement = false;
        lastHorizontalVelocity = Vector3.ProjectOnPlane(controller.velocity, Vector3.up);
        startTime = Time.time;
        startCooldownTime = Time.time;
        while (Time.time < startTime + dashTime)
        {
            float deltaModifier = (dashTime - (Time.time - startTime)) / dashTime;
            movement.Move(lastHorizontalVelocity.normalized * dashSpeed * (Time.deltaTime * deltaModifier));
            yield return null;
        }
        isDashing = false;
        startCooldownTime = Time.time;
        lastHorizontalVelocity = Vector3.zero;
        // movement.canInputMovement = true;
        StartCoroutine(DashCooldown());
    }

    IEnumerator DashCooldown()
    {
        canDash = false;
        yield return new WaitForSeconds(dashCoolTime);
        canDash = true;
    }
}
