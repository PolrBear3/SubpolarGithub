using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceTycoon_Main_GameController : MonoBehaviour
{
    // UI Static Bool
    public static bool isGroundOptionMenuOn = false;
    public static bool isWallOptionMenuOn = false;
    public static bool isPanelMenuOn = false;

    // Object Icon Constructors
    public void Icon_Popup_UpdateCheck(bool playerDetection, GameObject objectIcon)
    {
        // icon on condition
        if (playerDetection && !isPanelMenuOn)
        {
            objectIcon.SetActive(true);
        }
        // icon off condition
        if (!playerDetection || isPanelMenuOn)
        {
            objectIcon.SetActive(false);
        }
    }
    public void Icon_Press(GameObject objectIcon)
    {
        objectIcon.SetActive(true);
        isPanelMenuOn = true;
    }

    // Object Main Panel Constructors
    public void Manual_TurnOff(GameObject objectMenu)
    {
        objectMenu.SetActive(false);
        isPanelMenuOn = false;
    }
    public void Automatic_TurnOff(bool playerDetection, GameObject objectMenu)
    {
        if (!playerDetection && objectMenu.activeSelf)
        {
            objectMenu.SetActive(false);
            isPanelMenuOn = false;
        }
    }

    // Engines
    public static int EnginesOn = 0;
    void Engines_Min_Set()
    {
        if (EnginesOn <= 0)
        {
            EnginesOn = 0;
        }
    }
    public void Engine_On_Off(GameObject onButton, GameObject offButton, GameObject engineLight)
    {
        // engine on
        if (offButton.activeSelf)
        {
            onButton.SetActive(true);
            engineLight.SetActive(true);
            EnginesOn += 1;
        }
        // engine off
        if (onButton.activeSelf)
        {
            offButton.SetActive(true);
            engineLight.SetActive(false);
            EnginesOn -= 1;
        }
        Engines_Min_Set();
    }

    // craftable object storage
    public static int chairBed_storage = 0;
    public static int closet_storage = 0;
}
