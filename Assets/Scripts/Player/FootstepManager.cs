using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootstepManager : MonoBehaviour
{

    public AudioClip[] footstepsSFX;
    public AudioSource[] audioSourcePool;
    private int iterator;
    private int lastIndexPlayed;
    
    public void PlayFootstep()
    {
        audioSourcePool[iterator].PlayOneShot(nextFootstepSFX());
        iterator = (iterator + 1) % audioSourcePool.Length;
    }

    AudioClip nextFootstepSFX()
    {
        int index = Random.Range(0, footstepsSFX.Length);
        while(index == lastIndexPlayed)
        {
            index = Random.Range(0, footstepsSFX.Length);
        }
        lastIndexPlayed = index;
        return footstepsSFX[index];
        
    }
}
