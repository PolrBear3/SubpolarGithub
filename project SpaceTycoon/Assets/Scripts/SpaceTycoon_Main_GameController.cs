using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceTycoon_Main_GameController : MonoBehaviour
{
    private void Update()
    {
        Debug.Log("Panel Menu: " + isPanelMenuOn);
        Debug.Log("Ground option:" + isGroundOptionMenuOn);
    }

    public static bool isGroundOptionMenuOn = false;
    public static bool isWallOptionMenuOn = false;
    public static bool isPanelMenuOn = false;
}
