using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    public float lookSensitivity = 1f;
    public float triggerAxisThreshold = 0.4f;
    public bool invertYAxis = false;
    public bool invertXAxis = false;

    PlayerMovement playerMovement;
    bool fireInputWasHeld;
    bool fireInputWasPressed;

    private Gun gun;

    void Start()
    {
        gun = GetComponent<Gun>();
        playerMovement = GetComponent<PlayerMovement>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void LateUpdate()
    {
        fireInputWasHeld = GetFireInputHeld();
        fireInputWasPressed = GetFireInputDown();
        if (gun.automaticShooting)
        {
            if (fireInputWasHeld)
                gun.Shoot();
        }
        else
        {
            if (fireInputWasPressed)
            {
                gun.Shoot();
            }
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            gun.StartReload();
        }
    }

    public bool CanProcessInput()
    {
        return Cursor.lockState == CursorLockMode.Locked;
    }

    public Vector3 GetMoveInput()
    {
        if(CanProcessInput())
        {
            Vector3 move = new Vector3(Input.GetAxisRaw("Horizontal"), 0f, Input.GetAxisRaw("Vertical"));
            move = Vector3.ClampMagnitude(move, 1);
            return move;
        }
        return Vector3.zero;
    }

    public float GetLookInputsHorizontal()
    {
        return GetMouseOrStickLookAxis("Mouse X");
    }

    public float GetLookInputsVertical()
    {
        return GetMouseOrStickLookAxis("Mouse Y");
    }

    public bool GetJumpInputDown()
    {
        if(CanProcessInput())
        {
            return Input.GetButtonDown("Jump");
        }
        return false;
    }

    public bool GetJumpInputHeld()
    {
        if(CanProcessInput())
        {
            return Input.GetButton("Jump");
        }
        return false;
    }

    public bool GetFireInputDown()
    {
        return Input.GetButtonDown("Fire");
    }
    public bool GetFireInputReleased()
    {
        return Input.GetButtonUp("Fire");
    }

    public bool GetFireInputHeld()
    {
        if(CanProcessInput())
        {
            return Input.GetButton("Fire");
            /*
            bool isGamepad = Input.GetAxis("Gamepad Fire") != 0f;
            
            if(isGamepad)
            {
                return Input.GetAxis("Gamepad Fire") >= triggerAxisThreshold;
            }
            else
            {
                //return Input.GetButton("Fire");
            }
            */
            
        }
        return false;
    }

    public bool GetSprintInputHeld()
    {
        if(CanProcessInput())
        {
            return Input.GetButton("Sprint");
        }
        return false;
    }

    public bool GetCrouchInputDown()
    {
        if(CanProcessInput())
        {
            return Input.GetButtonDown("Crouch");
        }
        return false;
    }

    public bool GetCrouchInputReleased()
    {
        if(CanProcessInput())
        {
            return Input.GetButtonUp("Crouch");
        }
        return false;
    }

    float GetMouseOrStickLookAxis(string mouseInputName)
    {
        if(CanProcessInput())
        {
            float i = Input.GetAxisRaw(mouseInputName);

            // Handle inverting Y
            if(invertYAxis)
                i *= -1f;
            
            i *= lookSensitivity;

                i *= 0.01f;
            return i;
        }
        return 0f;
    }

}
