using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorSystem : MonoBehaviour
{
    public bool isOpened = false;

    [SerializeField] private List<Switch> listOfSwitches;
    [SerializeField] private GameObject door;

    public void SwitchCheck()
    {
        bool toBeOpened = true;
        foreach (Switch sw in listOfSwitches)
        {
            if (!sw.isActivated)
                toBeOpened = false;
        }
        OpenDoor(toBeOpened);
    }

    public void OpenDoor(bool state)
    {
        //open the door
        if (state)
            door.SetActive(false);
        else
            door.SetActive(true);

        //this is for monitoring in inspector
        isOpened = state;
    }

    public void RegisterSwitch(Switch sw)
    {
        listOfSwitches.Add(sw);
    }
}
