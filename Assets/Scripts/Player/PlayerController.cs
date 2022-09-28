using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public GameObject hitIndicator;
    public AudioSource playerAudio;
    public AudioClip hitIndicatorSFX;

    public IEnumerator PlayHitIndicator()
    {
        // TODO: Add an argument for type of hit
        hitIndicator.SetActive(true);
        playerAudio.PlayOneShot(hitIndicatorSFX);
        yield return new WaitForSeconds(0.05f);
        hitIndicator.SetActive(false);
    }
}
