using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EscapePod_Closet_Panel : MonoBehaviour
{
    private void Update()
    {
        Icon_Popup();
    }

    public GameObject gameObject_closet;
    public EscapePod_Closet_MainController controller;

    void TurnOff_All_Options()
    {
        controller.innerWearOption.SetActive(false);
        controller.spaceSuitOption.SetActive(false);
    }
    
    // icon
    void Icon_Popup()
    {
        // on
        if (controller.playerDetection && SpaceTycoon_Main_GameController.isPanelMenuOn == false)
        {
            controller.icon.SetActive(true);
        }
        // off
        if (controller.playerDetection == false || SpaceTycoon_Main_GameController.isPanelMenuOn)
        {
            controller.icon.SetActive(false);
        }
    }

    // icon pressed
    public void Icon_Pressed()
    {
        controller.mainPanel.SetActive(true);
        SpaceTycoon_Main_GameController.isPanelMenuOn = true;
    }

    // basic options
    // exit
    // dismantle

    // innerWear
    // option
    // select

    // spaceSuit
    // option
    // craft
    // select
}
