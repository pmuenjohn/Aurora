using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(CharacterController), typeof(PlayerInput))]
public class PlayerMovement : MonoBehaviour
{
    [Header("References")]
    public GameObject cameraHolder;

    [Header("General")]
    public float gravityDownForce = 20f;
    public LayerMask groundCheckLayers = -1;
    public float groundCheckDistance = 0.05f;

    [Header("Movement")]
    public float maxGroundSpeed = 10f;
    public float decelerationOnGround = 15f;
    public float crouchMultiplier = 0.5f;
    public float maxAirSpeed = 10f;
    public float airAcceleration = 25f;
    public float sprintMultiplier = 1.5f;

    [Header("Rotation")]
    public float rotationSpeed = 200f;
    [Range(0.1f, 1f)]
    public float ADSRotationMultiplier = 0.4f;
    public float camMinY = -80f, camMaxY = 89f;

    [Header("Jump")]
    public float jumpForce = 9f;

    [Header("Stance")]
    public UnityAction<bool> OnStanceChanged;
    public float camHeightRatio = 0.9f;
    public float capsuleHeightStanding = 1.8f;
    public float capsultHeightCrouching = 0.9f;
    public float crouchSpeed = 10f;

    // TODO: Port over Fall Damage calculations

    // TODO: Port over Audio 

    public Vector3 CharacterVelocity { get; set; }
    public bool IsGrounded;
    public bool HasJumpedThisFrame { get; set; }
    public bool IsCrouching { get; set; }

    public float speedKmh;

    public float RotationMultiplier
    {
        get
        {
            // TODO: return ADSRotationMultiplier if aiming
            return 1f;
        }
    }

    PlayerInput inputHandler;
    CharacterController controller;
    Vector3 groundNormal;
    Vector3 latestImpactSpeed;
    
    float lastTimeJumped = 0f;
    float camVerticalAngle = 0f;
    float targetCharacterHeight;

    const float GroundingPreventionTime = 0.2f;
    const float GroundCheckDistanceInAir = 0.07f;

    Wallrun wallRunComponent;
    

    void Start()
    {
        inputHandler = GetComponent<PlayerInput>();
        
        controller = GetComponent<CharacterController>();
        controller.enableOverlapRecovery = true;

        wallRunComponent = GetComponent<Wallrun>();

        SetCrouchingState(false, true);
        UpdateCharacterHeight(true);
    }

    void Update()
    {
        speedKmh = Vector3.ProjectOnPlane(controller.velocity, Vector3.up).magnitude * 3.6f;

        HasJumpedThisFrame = false;

        bool wasGrounded = IsGrounded;
        GroundCheck();

        // Landing
        if(IsGrounded && !wasGrounded)
        {
            //TODO: Apply Fall Damage and Audio
        }

        // Crouching
        if(inputHandler.GetCrouchInputDown())
        {
            SetCrouchingState(!IsCrouching, false);
        }

        UpdateCharacterHeight(false);
        HandleCharacterMovement();

    }

    void HandleCharacterMovement()
    {
        // Horizontal character rotation
        transform.Rotate(
            new Vector3(0f, (inputHandler.GetLookInputsHorizontal() * rotationSpeed * RotationMultiplier), 0f), Space.Self
        );

        // vertical camera rotation
        // add vertical inputs to the camera's vertical angle
        camVerticalAngle -= inputHandler.GetLookInputsVertical() * rotationSpeed * RotationMultiplier;

        // limit the camera's vertical angle to min/max
        camVerticalAngle = Mathf.Clamp(camVerticalAngle, -89f, 89f);

        // apply the vertical angle as a local rotation to the camera transform along its right axis (makes it pivot up and down)
        if(wallRunComponent != null)
        {
            cameraHolder.transform.localEulerAngles = new Vector3(camVerticalAngle, 0, wallRunComponent.GetCameraRoll());
        }
        else
        {
            cameraHolder.transform.localEulerAngles = new Vector3(camVerticalAngle, 0, 0);
        }

        // Character movement handling
        bool isSprinting = inputHandler.GetSprintInputHeld();
        
        if(isSprinting)
        {
            isSprinting = SetCrouchingState(false, false);
        }
        
        float speedModifier = isSprinting ? sprintMultiplier : 1f;

        Vector3 worldspaceMoveInput = transform.TransformVector(inputHandler.GetMoveInput());

        // Handle grounded movement
        if(IsGrounded || (wallRunComponent != null && wallRunComponent.IsWallRunning()))
        {
            if(IsGrounded)
            {
                // Calculate the desired velocity from inputs, max speed and current slope
                Vector3 targetVelocity = worldspaceMoveInput * maxGroundSpeed * speedModifier;
                // Reduce speed if crouching by crouch multiplier
                if(IsCrouching)
                    targetVelocity *= crouchMultiplier;
                targetVelocity = GetDirectionReorientedOnSlope(targetVelocity.normalized, groundNormal) * targetVelocity.magnitude;

                // Smoothly interpolate between our current velocity and the target velocity based on acceleration
                CharacterVelocity = Vector3.Lerp(CharacterVelocity, targetVelocity, decelerationOnGround * Time.deltaTime);
            }

            // Jumping
            if ((IsGrounded || (wallRunComponent != null && wallRunComponent.IsWallRunning())) && inputHandler.GetJumpInputDown())
            {
                // Force crouch state to false
                if(SetCrouchingState(false, false))
                {
                    if(IsGrounded)
                    {
                        // Start by cancelling out the vertical component of our velocity
                        CharacterVelocity = new Vector3(CharacterVelocity.x, 0f, CharacterVelocity.z);

                        // Then add jumpForce value upwards
                        CharacterVelocity += Vector3.up * jumpForce;
                    }
                    else
                    {
                        CharacterVelocity = new Vector3(CharacterVelocity.x, 0f, CharacterVelocity.z);
                        CharacterVelocity += wallRunComponent.GetWallJumpDirection() * jumpForce;
                    }
                    // TODO: Play Jump sound

                    // Remember the last time we jumped to prevent snapping to the ground for a short time
                    lastTimeJumped = Time.time;
                    HasJumpedThisFrame = true;

                    // Force grounding to false
                    IsGrounded = false;
                    groundNormal = Vector3.up;
                }
                
                // TODO: Play footstep sounds
            }

        }
        else
        {
            if(wallRunComponent == null || (wallRunComponent!= null && !wallRunComponent.IsWallRunning()))
            {   
                // Add air acceleration
                CharacterVelocity += worldspaceMoveInput * airAcceleration * Time.deltaTime;

                // Limit air speed to a maximum, but only horizontally
                float verticalVelocity = CharacterVelocity.y;
                Vector3 horizontalVelocity = Vector3.ProjectOnPlane(CharacterVelocity, Vector3.up);
                horizontalVelocity = Vector3.ClampMagnitude(horizontalVelocity, maxAirSpeed * speedModifier);
                CharacterVelocity = horizontalVelocity + (Vector3.up * verticalVelocity);
                
                // Apply gravity to the velocity
                CharacterVelocity += Vector3.down * gravityDownForce * Time.deltaTime;
            }
        }

        // Apply final calculated velocity value as character movement

        Vector3 capsuleBottomBeforeMove = GetCapsuleBottomHemisphere();
        Vector3 capsuleTopBeforeMove = GetCapsuleTopHemisphere(controller.height);
        controller.Move(CharacterVelocity * Time.deltaTime);

        // Detect obstructions to adjust velocity accordingly
        latestImpactSpeed = Vector3.zero;
        if (Physics.CapsuleCast(capsuleBottomBeforeMove, capsuleTopBeforeMove, controller.radius,
            CharacterVelocity.normalized, out RaycastHit hit, CharacterVelocity.magnitude * Time.deltaTime, -1,
            QueryTriggerInteraction.Ignore))
        {
            // We remember the last impact speed because the fall damage logic might need it
            latestImpactSpeed = CharacterVelocity;

            CharacterVelocity = Vector3.ProjectOnPlane(CharacterVelocity, hit.normal);
        }
    }

    void GroundCheck()
    {
        // Make sure that grounch check distance whilst in the air is small to prevent snapping to ground
        float chosenGroundCheckDistance = IsGrounded ? (controller.skinWidth + groundCheckDistance) : GroundCheckDistanceInAir;

        // Reset values before the groundCheck
        IsGrounded = false;
        groundNormal = Vector3.up;

        // Only try to detect ground if it's been a short amount of time since the last jump, toherwise we may snap to the ground after we try jumping
        if(Time.time >= lastTimeJumped + GroundingPreventionTime)
        {
            if(Physics.CapsuleCast(GetCapsuleBottomHemisphere(), GetCapsuleTopHemisphere(controller.height), 
            controller.radius, Vector3.down, out RaycastHit hit, chosenGroundCheckDistance, groundCheckLayers, 
            QueryTriggerInteraction.Ignore))
            {
                // Store the upward direction of the surface found
                groundNormal = hit.normal;

                // Only consider this a valid ground hit if the normal goes in the same direction as the character's up
                // and if the slope angle is lower than the controller's limit
                if(Vector3.Dot(hit.normal, transform.up) > 0f && IsNormalUnderSlopeLimit(groundNormal))
                {
                    IsGrounded = true;

                    // Handle snapping to the ground
                    if(hit.distance > controller.skinWidth)
                    {
                        controller.Move(Vector3.down * hit.distance);
                    }
                }
            }
        }
    }

    bool IsNormalUnderSlopeLimit(Vector3 normal)
    {
        return Vector3.Angle(transform.up, normal) <= controller.slopeLimit;
    }

    Vector3 GetCapsuleBottomHemisphere()
    {
        return transform.position + (transform.up * controller.radius);
    }
    Vector3 GetCapsuleTopHemisphere(float atHeight)
    {
        return transform.position + (transform.up * (atHeight - controller.radius));
    }

    // Gets a reoriented direction that is tangent to a given slope
    public Vector3 GetDirectionReorientedOnSlope(Vector3 direction, Vector3 slopeNormal)
    {
        Vector3 directionRight = Vector3.Cross(direction, transform.up);
        return Vector3.Cross(slopeNormal, directionRight).normalized;
    }

    void UpdateCharacterHeight(bool force)
    {
        // Update height instantly
        if(force)
        {
            controller.height = targetCharacterHeight;
            controller.center = Vector3.up * controller.height * 0.5f;
            cameraHolder.transform.localPosition = Vector3.up * targetCharacterHeight * camHeightRatio;
            // TODO: Adjust enemy aiming point to controller.center
        } 
        // Update height smoothly
        else if (controller.height != targetCharacterHeight)
        {
            controller.height = Mathf.Lerp(controller.height, targetCharacterHeight,
                crouchSpeed * Time.deltaTime);
            controller.center = Vector3.up * controller.height * 0.5f;
            cameraHolder.transform.localPosition = Vector3.Lerp(cameraHolder.transform.localPosition,
                Vector3.up * targetCharacterHeight * camHeightRatio, crouchSpeed * Time.deltaTime);
            // TODO: Adjust enemy aiming point to controller.center
        }
    }

    // Returns false if there is an obstruction
    bool SetCrouchingState(bool crouched, bool ignoreObstructions)
    {
        // Set appropriate heights
        if(crouched)
        {
            targetCharacterHeight = capsultHeightCrouching;
        }
        else
        {
            // Detect obstructions
            if(!ignoreObstructions)
            {
                Collider[] standingOverlaps = Physics.OverlapCapsule(
                    GetCapsuleBottomHemisphere(),
                    GetCapsuleTopHemisphere(capsuleHeightStanding),
                    controller.radius,
                    -1,
                    QueryTriggerInteraction.Ignore);
                foreach (Collider c in standingOverlaps)
                {
                    if (c != controller)
                    {
                        return false;
                    }
                }
            }

            targetCharacterHeight = capsuleHeightStanding;
        }

        if(OnStanceChanged != null)
        {
            OnStanceChanged.Invoke(crouched);
        }

        IsCrouching = crouched;
        return true;
    }


    private GUIStyle guiStyle = new GUIStyle();
    void OnGUI()
    {
        guiStyle.fontSize = 50;
        GUI.Label(new Rect(10, 10, 100, 90), Mathf.Round(speedKmh).ToString(), guiStyle);
    }

}
