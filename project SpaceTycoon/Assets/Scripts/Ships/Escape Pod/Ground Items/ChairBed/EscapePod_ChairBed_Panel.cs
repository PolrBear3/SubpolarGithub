using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EscapePod_ChairBed_Panel : MonoBehaviour
{
    public GameObject EscapePod_ChairBed_gameObject;
    public EscapePod_ChairBed_MainController controller;

    private void Update()
    {
        Icon_Popup();
        Automatic_Off();
        Mode_Check_for_Button();
    }

    // icon
    void Icon_Popup()
    {
        if (controller.playerDetection == true && SpaceTycoon_Main_GameController.isPanelMenuOn == false)
        {
            controller.icon.SetActive(true);
            SpaceTycoon_Main_GameController.isPanelMenuOn = true;
        }
        if (controller.playerDetection == false || controller.mainPanel.activeSelf == true)
        {
            controller.icon.SetActive(false);
            SpaceTycoon_Main_GameController.isPanelMenuOn = false;
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
        if (controller.playerDetection == false)
        {
            controller.mainPanel.SetActive(false);
            SpaceTycoon_Main_GameController.isPanelMenuOn = false;
        }
    }
    
    // functions
    private bool facingLeft = false;
    public void Rotate()
    {
        facingLeft = !facingLeft;
        EscapePod_ChairBed_gameObject.transform.Rotate(0f, 180f, 0f);
    }

    public void Dismantle()
    {
        Destroy(EscapePod_ChairBed_gameObject);
        // give back chairBed ingredients to player
    }

    void Mode_Check_for_Button()
    {
        if (controller.chairMode == true)
        {
            controller.chairModeButton.SetActive(false);
            controller.bedModeButton.SetActive(true);
        }
        if (controller.chairMode == false)
        {
            controller.chairModeButton.SetActive(true);
            controller.bedModeButton.SetActive(false);
        }
    }

    public void Change_to_Bed()
    {
        controller.sr.sprite = controller.bed;
        controller.chairMode = false;
    }

    public void Change_to_Chair()
    {
        controller.sr.sprite = controller.chair;
        controller.chairMode = true;
    }
}
