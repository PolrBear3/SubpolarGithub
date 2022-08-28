using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Icon_Points_Manager : MonoBehaviour
{
    private void Update()
    {
        Icon_Point_Empty_Check();
    }

    public Icon_Point point1;
    public static bool point1Empty = true;
    public Icon_Point point2;
    public static bool point2Empty = true;
    public Icon_Point point3;
    public static bool point3Empty = true;
    public Icon_Point point4;
    public static bool point4Empty = true;
    public Icon_Point point5;
    public static bool point5Empty = true;

    void Icon_Point_Empty_Check()
    {
        // 1
        if (point1.iconDetection == true)
        {
            point1Empty = false;
        }
        else if (point1.iconDetection == false)
        {
            point1Empty = true;
        }
        // 2
        if (point2.iconDetection == true)
        {
            point2Empty = false;
        }
        else if (point2.iconDetection == false)
        {
            point2Empty = true;
        }
        // 3
        if (point3.iconDetection == true)
        {
            point3Empty = false;
        }
        else if (point3.iconDetection == false)
        {
            point3Empty = true;
        }
        // 4
        if (point4.iconDetection == true)
        {
            point4Empty = false;
        }
        else if (point4.iconDetection == false)
        {
            point4Empty = true;
        }
        // 5
        if (point5.iconDetection == true)
        {
            point5Empty = false;
        }
        else if (point5.iconDetection == false)
        {
            point5Empty = true;
        }
    }
}
