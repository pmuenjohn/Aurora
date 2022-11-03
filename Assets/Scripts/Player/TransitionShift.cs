using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitionShift : MonoBehaviour
{
    public Animator anim;

    public void PlayTransition()
    {
        anim.SetTrigger("play");
    }
}
