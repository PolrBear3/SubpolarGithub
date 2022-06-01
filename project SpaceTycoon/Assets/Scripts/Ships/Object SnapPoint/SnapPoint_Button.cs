using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SnapPoint_Button : MonoBehaviour
{
    [HideInInspector]
    public Button button;
    [HideInInspector]
    public bool buttonPressed = false;

    private void Awake()
    {
        button = GetComponent<Button>();
    }

    public void Button_Press()
    {
        buttonPressed = true;
    }
    public void Set_Backto_UnPressed()
    {
        buttonPressed = false;
    }
}
