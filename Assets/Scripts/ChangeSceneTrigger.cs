using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeSceneTrigger : MonoBehaviour
{
    public string nextSceneName;
    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            PlayerStatus.hasCurrentCheckpoint = false;
            SceneManager.LoadScene(nextSceneName);
        }
    }
}
