using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HPCount : MonoBehaviour
{
    public Image hpFill;
    public PlayerStatus player;

    // Update is called once per frame
    void Update()
    {
        hpFill.fillAmount = player.hp / player.maxHP;
    }
}
