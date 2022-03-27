using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceTycoon_Main_GameController : MonoBehaviour
{
    private void Update()
    {
        Engines_Min_Set();
    }

    // UI check
    public static bool isGroundOptionMenuOn = false;
    public static bool isWallOptionMenuOn = false;
    public static bool isPanelMenuOn = false;

    // ship speed
    public static int EnginesOn = 0;
    void Engines_Min_Set()
    {
        if (EnginesOn <= 0)
        {
            EnginesOn = 0;
        }
    }
}
