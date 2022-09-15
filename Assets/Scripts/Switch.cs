using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Switch : MonoBehaviour
{
    public bool isActivated = false;
    public Material onMaterial;
    public Material offMaterial;
    private MeshRenderer meshRenderer;

    private void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
    }

    public void ChangeActivationState(bool stateIsActivated)
    {
        isActivated = stateIsActivated;
        meshRenderer.material = (isActivated ? onMaterial : offMaterial);
    }
}
