using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class PlayerCanvas : MonoBehaviour
{
    public TextMeshProUGUI dimensionText;
    public TimeTraveller timeTraveller;

    [Header("References")]
    public GameObject HUD;
    public GameObject deathScreen;

    private void Start()
    {
        SetUpHUD();
    }

    // Update is called once per frame
    void Update()
    {
        if (timeTraveller.inDimension2)
        {
            dimensionText.text = "Dimension 2";
        }
        else
        {
            dimensionText.text = "Dimension 1";
        }
    }

    public void DisplayDeathScreen()
    {
        Time.timeScale = 0f;
        deathScreen.SetActive(true);
        HUD.SetActive(false);
        Cursor.lockState = CursorLockMode.None;
    }

    public void SetUpHUD()
    {
        HUD.SetActive(true);
        Cursor.lockState = CursorLockMode.Locked;
        deathScreen.SetActive(false);
    }

    public void Respawn()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        Debug.Log(Time.timeScale);
    }
}
