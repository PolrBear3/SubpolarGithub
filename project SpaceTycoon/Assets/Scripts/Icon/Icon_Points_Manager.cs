using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Icon_Points_Manager : MonoBehaviour
{
    private void Update()
    {
        Icon_Point_Empty_Check();
        Debug.Log(point2Empty);
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
    public Icon_Point point6;
    public static bool point6Empty = true;
    public Icon_Point point7;
    public static bool point7Empty = true;
    public Icon_Point point8;
    public static bool point8Empty = true;
    public Icon_Point point9;
    public static bool point9Empty = true;
    public Icon_Point point10;
    public static bool point10Empty = true;
    public Icon_Point point11;
    public static bool point11Empty = true;

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
        // 6
        if (point6.iconDetection == true)
        {
            point6Empty = false;
        }
        else if (point6.iconDetection == false)
        {
            point6Empty = true;
        }
        // 7
        if (point7.iconDetection == true)
        {
            point7Empty = false;
        }
        else if (point7.iconDetection == false)
        {
            point7Empty = true;
        }
        // 8
        if (point8.iconDetection == true)
        {
            point8Empty = false;
        }
        else if (point8.iconDetection == false)
        {
            point8Empty = true;
        }
        // 9
        if (point9.iconDetection == true)
        {
            point9Empty = false;
        }
        else if (point9.iconDetection == false)
        {
            point9Empty = true;
        }
        // 10
        if (point10.iconDetection == true)
        {
            point10Empty = false;
        }
        else if (point10.iconDetection == false)
        {
            point10Empty = true;
        }
        // 11
        if (point11.iconDetection == true)
        {
            point11Empty = false;
        }
        else if (point11.iconDetection == false)
        {
            point11Empty = true;
        }
    }
}
