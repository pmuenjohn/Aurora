using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DashIndicator : MonoBehaviour
{
    
    public Dash dash;
    public Color cooldownColor;
    public Color readyColor;

    public Image dashFill;

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!dash.canDash)
        {
            dashFill.color = cooldownColor;
            if (!dash.isDashing)
            {
                dashFill.fillAmount = (Time.time - dash.startCooldownTime) / dash.dashCoolTime;
            }
        }
        else
        {
            if (dash.isDashing)
            {
                dashFill.fillAmount = 0f;
            }
            else
            {
                dashFill.color = readyColor;
                dashFill.fillAmount = 1f;
            }
        }
    }
}
