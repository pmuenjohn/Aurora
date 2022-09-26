using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Switch : MonoBehaviour
{
    public bool isActivated = false;
    public Material onMaterial;
    public Material offMaterial;
    private MeshRenderer meshRenderer;
    private DoorSystem doorConnection;

    private void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        doorConnection = transform.parent.gameObject.GetComponent<DoorSystem>();
        if (doorConnection != null)
            doorConnection.RegisterSwitch(this);
        else
            Destroy(gameObject);
    }

    public void ChangeActivationState(bool stateIsActivated)
    {
        isActivated = stateIsActivated;
        meshRenderer.material = (isActivated ? onMaterial : offMaterial);
        doorConnection.SwitchCheck();
    }
}
