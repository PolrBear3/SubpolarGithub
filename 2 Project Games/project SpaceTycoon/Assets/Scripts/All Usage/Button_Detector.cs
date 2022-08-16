using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Button_Detector : MonoBehaviour
{
    [HideInInspector]
    public bool buttonPressed = false;

    public void Button_Press()
    {
        buttonPressed = true;
    }
    public void Set_Backto_UnPressed()
    {
        buttonPressed = false;
    }

    // how to use Button_Detector
    // 1. attach this script to button
    // 2. set On Click Runtime to Button_Press()
    // 3. at other button controller script, use it as bool ex) if (gameButton.buttonPressed == true)
    // 4. put all the functions you want for when button is pressed
    // 5. after all the functions are made, set the button back to false ex) gameButton.Set_Backto_UnPressed();
}
