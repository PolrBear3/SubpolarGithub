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
    
    public static bool Shipspeed0;
    [HideInInspector]
    public static bool Shipspeed1;
    [HideInInspector]
    public static bool Shipspeed2;
    [HideInInspector]
    public static bool Shipspeed3;

    void Reset_Speed()
    {
        Shipspeed0 = false;
        Shipspeed1 = false;
        Shipspeed2 = false;
        Shipspeed3 = false;
    }

    void Enignes_Amount_Speed()
    {
        if (EnginesOn == 0)
        {
            Reset_Speed();
            Shipspeed0 = true;
        }
        if (EnginesOn == 1)
        {
            Reset_Speed();
            Shipspeed1 = true;
        }
        if (EnginesOn == 2)
        {
            Reset_Speed();
            Shipspeed2 = true;
        }
        if (EnginesOn == 3)
        {
            Reset_Speed();
            Shipspeed3 = true;
        }
    }
}
