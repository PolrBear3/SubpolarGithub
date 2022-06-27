using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class snapPoint_Buttons : MonoBehaviour
{
    public Button button;
    
    public GameObject availableButtonImage;
    public GameObject unAvailableButtonImage;

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

    public void SnapPoint_Button_Available()
    {
        availableButtonImage.SetActive(true);
        unAvailableButtonImage.SetActive(false);
        button.enabled = true;
    }

    public void SnapPoint_Button_UnAvailable()
    {
        availableButtonImage.SetActive(false);
        unAvailableButtonImage.SetActive(true);
        button.enabled = false;
    }
}
