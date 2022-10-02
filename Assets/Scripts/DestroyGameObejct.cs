using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyGameObejct : MonoBehaviour
{
    public float destroyAfterSeconds = 0.5f;

    void Start()
    {
        Destroy(this.gameObject, destroyAfterSeconds);
    }
}
