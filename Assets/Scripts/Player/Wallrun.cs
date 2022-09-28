using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

public class Wallrun : MonoBehaviour
{
    public Vector3 wallRunRaycastOffset;
    public float wallMaxDistance = 1f;
    public float wallSpeedMultiplier = 8.5f;
    public float minimumHeight = 0.7f;
    public float maxAngleRoll = 20f;
    [Range(0.0f ,1.0f)]
    public float normalizedAngleThreshold = 0.3f;

    public float jumpDuration = 0.25f;
    public float wallRunDuration = 3f;
    public float wallRunCooldown = 1f;
    public float wallBouncing = 3f;
    public float camTransitionDuration = 1f;

    public float wallGravityDownForce = 20f;
    public float boostUpwardForce = 20f;
    public float boostGracePeriod = 0.5f;
    public float boostTime = 0.5f;

    public bool useSprint;

    [Space]
    public Volume postProcessVolume;

    PlayerMovement movement;
    PlayerInput inputHandler;

    public Vector3[] directions;
    RaycastHit[] hits;
    [SerializeField] LayerMask layerMask;

    bool isWallRunning = false;
    Vector3 lastWallPosition;
    Vector3 lastWallNormal;
    public float elapsedTimeSinceJump = 0;
    public float elapsedTimeSinceKickoff = 0;
    float elapsedTimeSinceWallAttach = 0;
    float elapsedTimeSinceWallDetach = 0;
    public bool jumping;
    public bool kickOff;
    float lastVolumeValue = 0f;

    bool isPlayerGrounded() => movement.IsGrounded;

    public bool IsWallRunning() => isWallRunning;

    bool CanWallRun()
    {
        float verticalAxis = Input.GetAxisRaw("Vertical");
        bool isSprinting = inputHandler.GetSprintInputHeld();
        isSprinting = !useSprint ? true : isSprinting;

        return !isPlayerGrounded() && verticalAxis > 0 && VerticalCheck() && isSprinting && elapsedTimeSinceWallAttach <= wallRunDuration;
    }

    bool VerticalCheck()
    {
        return !Physics.Raycast(transform.position, Vector3.down, minimumHeight);
    }

    bool CanAttach()
    {
        if(jumping)
        {
            elapsedTimeSinceJump += Time.deltaTime;
            if(elapsedTimeSinceJump > jumpDuration)
            {
                elapsedTimeSinceJump = 0;
                jumping = false;
            }
            return false;
        }
        return true;
    }

    bool WithinBoostGracePeriod()
    {
        return kickOff && elapsedTimeSinceKickoff < boostGracePeriod;
    }

    float CalculateSide()
    {
        if(isWallRunning)
        {
            Vector3 heading = lastWallPosition - transform.position;
            Vector3 perp = Vector3.Cross(transform.forward, heading);
            float dir = Vector3.Dot(perp, transform.up);
            return dir;
        }
        return 0;
    }

    public float GetCameraRoll()
    {
        float dir = CalculateSide();
        float cameraAngle = movement.cameraHolder.transform.eulerAngles.z;
        float targetAngle = 0;
        if(dir != 0)
        {
            targetAngle = Mathf.Sign(dir) * maxAngleRoll;
        }
        return Mathf.LerpAngle(cameraAngle, targetAngle, Mathf.Max(elapsedTimeSinceWallAttach, elapsedTimeSinceWallDetach) / camTransitionDuration);
    } 

    public Vector3 GetWallJumpDirection()
    {
        if(isWallRunning)
        {
            return lastWallNormal * wallBouncing + Vector3.up;
        }
        return Vector3.zero;
    }

    void Start()
    {
        movement = GetComponent<PlayerMovement>();
        inputHandler = GetComponent<PlayerInput>();

        directions = new Vector3[]{
            Vector3.right,
            Vector3.right + Vector3.forward,
            // Vector3.forward,
            Vector3.left + Vector3.forward,
            Vector3.left,
            
        };

        if(postProcessVolume != null)
        {
            SetVolumeWeight(0);
        }
    }

    void LateUpdate()
    {
        isWallRunning = false;

        if(inputHandler.GetJumpInputDown())
        {
            jumping = true;
            kickOff = true;
        }

        if(kickOff)
        {
            if(movement.IsGrounded)
            {
                kickOff = false;
                elapsedTimeSinceKickoff = 0;
            }
            else
            {
                elapsedTimeSinceKickoff += Time.deltaTime;
            }
        }

        if(CanAttach())
        {
            hits = new RaycastHit[directions.Length];
            for (int i = 0; i < directions.Length; i++)
            {
                Vector3 dir = transform.TransformDirection(directions[i]);
                Physics.Raycast(transform.position + wallRunRaycastOffset, dir, out hits[i], wallMaxDistance, layerMask);
                //DEBUG GIZMO RAY
                if(hits[i].collider != null)
                {
                    Debug.DrawRay(transform.position + wallRunRaycastOffset, dir * hits[i].distance, Color.green);
                }
                else
                {
                    Debug.DrawRay(transform.position + wallRunRaycastOffset, dir * wallMaxDistance, Color.red);
                }
            }
            if(CanWallRun())
            {
                hits = hits.ToList().Where(h => h.collider != null).OrderBy(h => h.distance).ToArray();
                if(hits.Length > 0)
                {
                    OnWall(hits[0]);
                    lastWallPosition = hits[0].point;
                    lastWallNormal = hits[0].normal;
                }
            }
        }

        if(isWallRunning)
        {
            elapsedTimeSinceWallDetach = 0;
            if(elapsedTimeSinceWallAttach == 0 && postProcessVolume != null)
            {
                lastVolumeValue = postProcessVolume.weight;
            }
            elapsedTimeSinceWallAttach += Time.deltaTime;
            float verticalStrength = WithinBoostGracePeriod() ? Mathf.Lerp(-boostUpwardForce, wallGravityDownForce, elapsedTimeSinceWallAttach / boostTime) : wallGravityDownForce;
            movement.CharacterVelocity += Vector3.down * verticalStrength * Time.deltaTime;
        }
        else
        {   
            elapsedTimeSinceWallAttach = 0;
            if(elapsedTimeSinceWallDetach == 0 && postProcessVolume != null)
            {
                lastVolumeValue = postProcessVolume.weight;
            }
            elapsedTimeSinceWallDetach += Time.deltaTime;
        }

        if(postProcessVolume != null)
        {
            HandleVolume();
        }
    }

    void OnWall(RaycastHit hit)
    {
        float d = Vector3.Dot(hit.normal, Vector3.up);
        if(d >= -normalizedAngleThreshold && d <= normalizedAngleThreshold)
        {
            // Vector3 alongWall = Vector3.Cross(hit.normal, Vector3.up);
            float vertical = Input.GetAxisRaw("Vertical");
            Vector3 alongWall = transform.TransformDirection(Vector3.forward);

            Debug.DrawRay(transform.position, alongWall.normalized * 10, Color.green);
            Debug.DrawRay(transform.position, lastWallNormal * 10, Color.magenta);

            movement.CharacterVelocity = alongWall * vertical * wallSpeedMultiplier;
            isWallRunning = true;
        }
    }

    void HandleVolume()
    {
        float w = 0;
        if(isWallRunning)
        {
            w = Mathf.Lerp(lastVolumeValue, 1, elapsedTimeSinceWallAttach / camTransitionDuration);
        } else
        {
            w = Mathf.Lerp(lastVolumeValue, 0, elapsedTimeSinceWallDetach / camTransitionDuration);
        }

        SetVolumeWeight(w);
    }
    
    void SetVolumeWeight(float weight)
    {
        postProcessVolume.weight = weight;
    }

}
