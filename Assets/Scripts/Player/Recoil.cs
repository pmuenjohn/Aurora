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
    public float c_initialSpeed;
    public float c_settleSpeed;

    private Vector3 c_currentRot;
    private Vector3 c_targetRot;
    
    [Header("Gun recoil (cm)")]
    public Vector3 g_kickback;
    public Vector3 g_tilt;
    public float g_initialSpeed;
    public float g_settleSpeed;

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

    void Start()
    {
        g_currentPos = g_defaultPos = gunPivot.localPosition;
    }

    void Update()
    {
        c_targetRot = Vector3.Lerp(c_targetRot, Vector3.zero, c_settleSpeed * Time.deltaTime);
        c_currentRot = Vector3.Slerp(c_currentRot, c_targetRot, c_initialSpeed * Time.fixedDeltaTime);
        recoilHolder.localRotation = Quaternion.Euler(c_currentRot);

        g_targetPos = Vector3.Lerp(g_targetPos, g_defaultPos, g_settleSpeed * Time.deltaTime);
        g_currentPos = Vector3.Lerp(g_currentPos, g_targetPos, g_initialSpeed * Time.fixedDeltaTime);
        gunPivot.localPosition = g_currentPos;
        
        g_targetTilt = Vector3.Lerp(g_targetTilt, Vector3.zero, g_settleSpeed * Time.deltaTime);
        g_currentTilt = Vector3.Slerp(g_currentTilt, g_targetTilt, g_initialSpeed * Time.fixedDeltaTime);
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
