using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AmmoCounter : MonoBehaviour
{
    public Gun gun;
    public TextMeshProUGUI text;

    // Update is called once per frame
    void FixedUpdate()
    {
        if (gun.reloading)
        {
            text.text = "RELOADING...";
        }
        else
            text.text = "Ammo:   " + gun.ammoLeft + " / " + gun.magazineSize;
    }
}
