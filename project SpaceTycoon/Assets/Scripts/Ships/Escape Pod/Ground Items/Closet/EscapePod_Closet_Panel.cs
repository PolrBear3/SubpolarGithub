using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EscapePod_Closet_Panel : MonoBehaviour
{
    private void Update()
    {
        Icon_Popup();
        Automatic_Exit();
    }

    public GameObject gameObject_closet;
    public EscapePod_Closet_MainController controller;

    public void TurnOff_All_Options()
    {
        controller.spaceSuitOptionPanel.SetActive(false);
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

    // exit menu
    public void Manual_Exit()
    {
        controller.mainPanel.SetActive(false);
        SpaceTycoon_Main_GameController.isPanelMenuOn = false;
    }
    void Automatic_Exit()
    {
        if (controller.playerDetection == false && controller.mainPanel.activeSelf == true)
        {
            controller.mainPanel.SetActive(false);
            SpaceTycoon_Main_GameController.isPanelMenuOn = false;
        }
    }

    // dismantle
    public void Dismantle()
    {
        controller.iconBoxCollider.SetActive(false);
        Destroy(gameObject_closet);
        // 1 EscapePod closet available in crafttable
    }

    // innerWear
        // select
    public void InnerWear_Select()
    {
        Player_Outfit.outfitNum = 1;
    }

    // spaceSuit
        // option menu
    public void SpaceSuit_Options()
    {
        controller.spaceSuitOptionPanel.SetActive(true);
    }
        // craft
    public void SpaceSuit_Craft()
    {
        // if player has all the ingredients
        TurnOff_All_Options();
        controller.spaceSuitOptionButton.SetActive(false);
        controller.spaceSuitSelectButton.SetActive(true);
    }
        // select
    public void SpaceSuit_Select()
    {
        Player_Outfit.outfitNum = 2;
    }
}
