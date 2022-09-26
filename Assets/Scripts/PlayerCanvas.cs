using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerCanvas : MonoBehaviour
{
    public TextMeshProUGUI dimensionText;
    public TimeTraveller timeTraveller;

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
}
