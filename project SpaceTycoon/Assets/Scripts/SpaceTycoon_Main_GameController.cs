using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceTycoon_Main_GameController : MonoBehaviour
{
    private void Update()
    {
        Enignes_Amount_Speed();
    }

    // UI check
    public static bool isGroundOptionMenuOn = false;
    public static bool isWallOptionMenuOn = false;
    public static bool isPanelMenuOn = false;

    // ship speed
    public static int EnginesOn = 0;
    
    public static bool shipSpeed0;
    [HideInInspector]
    public static bool shipSpeed1;
    [HideInInspector]
    public static bool shipSpeed2;
    [HideInInspector]
    public static bool shipSpeed3;

    void Reset_Speed()
    {
        shipSpeed0 = false;
        shipSpeed1 = false;
        shipSpeed2 = false;
        shipSpeed3 = false;
    }

    void Enignes_Amount_Speed()
    {
        if (EnginesOn == 0)
        {
            Reset_Speed();
            shipSpeed0 = true;
        }
        if (EnginesOn == 1)
        {
            Reset_Speed();
            shipSpeed1 = true;
        }
        if (EnginesOn == 2)
        {
            Reset_Speed();
            shipSpeed2 = true;
        }
        if (EnginesOn == 3)
        {
            Reset_Speed();
            shipSpeed3 = true;
        }
    }
}
