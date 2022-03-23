using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EscapePod_Engine_Panel : MonoBehaviour
{
    public EscapePod_Engine_Controller controller;
    
    private void Update()
    {
        Icon_Popup();
        Automatic_Off();
    }

    // icon
    void Icon_Popup()
    {
        // on
        if (controller.playerDetection == true && SpaceTycoon_Main_GameController.isPanelMenuOn == false)
        {
            controller.icon.SetActive(true);
        }
        // off
        if (controller.playerDetection == false || SpaceTycoon_Main_GameController.isPanelMenuOn == true)
        {
            controller.icon.SetActive(false);
        }
    }
    public void Icon_Pressed()
    {
        controller.mainPanel.SetActive(true);
        SpaceTycoon_Main_GameController.isPanelMenuOn = true;
    }

    // main panel
    public void Manual_Off()
    {
        controller.mainPanel.SetActive(false);
        SpaceTycoon_Main_GameController.isPanelMenuOn = false;
    }
    void Automatic_Off()
    {
        if (controller.playerDetection == false && controller.mainPanel.activeSelf == true)
        {
            controller.mainPanel.SetActive(false);
            SpaceTycoon_Main_GameController.isPanelMenuOn = false;
        }
    }
}
