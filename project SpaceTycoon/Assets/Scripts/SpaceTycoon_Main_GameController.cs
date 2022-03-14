using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceTycoon_Main_GameController : MonoBehaviour
{
    public static bool isGroundOptionMenuOn = false;
    public static bool isWallOptionMenuOn = false;
    public static bool isPanelMenuOn = false;

    public static int currentIconNumbers_EscapePod = 0;
    
    public Icon_Point point2;
    public static bool point2Empty = true;
    public Icon_Point point3;
    public static bool point3Empty = true;

    private void Update()
    {
        Debug.Log(point2Empty);
        
        if(point2.iconDetection == true)
        {
            point2Empty = false;
        }
        else if (point2.iconDetection == false)
        {
            point2Empty = true;
        }

        if (point3.iconDetection == true)
        {
            point3Empty = false;
        }
        else if (point3.iconDetection == false)
        {
            point3Empty = true;
        }
    }
}
