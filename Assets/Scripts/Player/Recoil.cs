using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Recoil script stolen from https://www.youtube.com/c/GilbertGames
public class Recoil : MonoBehaviour
{
    [Header("References")]
    public Transform recoilHolder;
    public Transform gunPivot;
    public Transform swayPivot;
    public PlayerInput inputHandler;
    public Cinemachine.CinemachineImpulseSource cameraShake;
    public Transform playerCamera;

    [Header("Camera recoil (degrees)")]
    public Vector3 c_recoil;
    public float c_initialSmoothTime;
    public float c_settleSmoothTime;

    private Vector3 c_targetRot;
    private Vector3 c_currentRot;

    private Vector3 c_targetVel =  Vector3.zero;
    private Vector3 c_currentVel =  Vector3.zero;
    
    [Header("Gun recoil (cm)")]
    public Vector3 g_kickback;
    public Vector3 g_tilt;
    public float g_initialSmoothTime;
    public float g_settleSmoothTime;

    private Vector3 g_targetVel =  Vector3.zero;
    private Vector3 g_currentVel =  Vector3.zero;

    private Vector3 g_targetTiltVel =  Vector3.zero;
    private Vector3 g_currentTiltVel =  Vector3.zero;

    private Vector3 g_currentPos;
    private Vector3 g_targetPos;
    private Vector3 g_defaultPos;
    private Vector3 g_currentTilt;
    private Vector3 g_targetTilt;

    [Header("Gun sway")]
    public float s_smooth;
    public float s_inputMultiplier;
    private float s_horizontal;
    private float s_vertical;

    private Vector2 s_velocity = Vector2.zero;

    

    void Start()
    {
        g_currentPos = g_defaultPos = gunPivot.localPosition;
    }

    void Update()
    {
        c_targetRot = Vector3.SmoothDamp(c_targetRot, Vector3.zero, ref c_targetVel, c_settleSmoothTime);
        c_currentRot = Vector3.SmoothDamp(c_currentRot, c_targetRot, ref c_currentVel, c_initialSmoothTime);
        recoilHolder.localRotation = Quaternion.Euler(c_currentRot);

        g_targetPos = Vector3.SmoothDamp(g_targetPos, g_defaultPos, ref g_targetVel, g_settleSmoothTime);
        g_currentPos = Vector3.SmoothDamp(g_currentPos, g_targetPos, ref g_currentVel, g_initialSmoothTime);
        gunPivot.localPosition = g_currentPos;
        
        g_targetTilt = Vector3.SmoothDamp(g_targetTilt, Vector3.zero, ref g_targetTiltVel, g_settleSmoothTime);
        g_currentTilt = Vector3.SmoothDamp(g_currentTilt, g_targetTilt, ref g_currentTiltVel, g_initialSmoothTime);
        gunPivot.localRotation = Quaternion.Euler(g_currentTilt);
    }

    void LateUpdate()
    {
         s_horizontal = Mathf.Lerp(s_horizontal, Input.GetAxisRaw("Mouse X") * s_inputMultiplier, Time.fixedDeltaTime * 5f);
         s_vertical = Mathf.Lerp(s_vertical, Input.GetAxisRaw("Mouse Y") * s_inputMultiplier, Time.fixedDeltaTime * 5f);
        
        Quaternion rotationX = Quaternion.AngleAxis(-s_vertical, Vector3.right);
        // Quaternion rotationY = Quaternion.AngleAxis(s_horizontal, Vector3.up);
        Quaternion rotationZ = Quaternion.AngleAxis(-s_horizontal, Vector3.forward);

        Quaternion targetRotation = rotationX * rotationZ;

        swayPivot.localRotation = Quaternion.Slerp(swayPivot.localRotation, targetRotation, s_smooth * Time.fixedDeltaTime);
    }

    public void GenerateRecoil()
    {
        cameraShake.GenerateImpulse(playerCamera.transform.forward);
        c_targetRot += new Vector3(c_recoil.x, Random.Range(-c_recoil.y, c_recoil.y), Random.Range(-c_recoil.z, c_recoil.z));
        g_targetPos += 0.01f * new Vector3(Random.Range(-g_kickback.x, g_kickback.x), g_kickback.y, Random.Range(0, g_kickback.z));
        g_targetTilt += new Vector3(-g_tilt.x, Random.Range(-g_tilt.y, g_tilt.y), Random.Range(-g_tilt.z, g_tilt.z));
    }
}
