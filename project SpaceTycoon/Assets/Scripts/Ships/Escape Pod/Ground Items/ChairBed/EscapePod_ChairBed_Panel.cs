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
        // is on 
        if (controller.playerDetection == true && SpaceTycoon_Main_GameController.isPanelMenuOn == false)
        {
            controller.icon.SetActive(true);
        }
        // is off
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

    private int iconOrderNumber;
    void icon_Order_Set()
    {
        if (SpaceTycoon_Main_GameController.numberOfIconsOn == 0)
        {
            iconOrderNumber = 1;
            SpaceTycoon_Main_GameController.numberOfIconsOn += 1;
        }
        else if (SpaceTycoon_Main_GameController.numberOfIconsOn == 0)
        {
            iconOrderNumber = 2;
            SpaceTycoon_Main_GameController.numberOfIconsOn += 1;
        }
    }
    void Set_Icon_Position()
    {
        if (iconOrderNumber == 1)
        {
            controller.icon.transform.localPosition = new Vector2(-140, -120);
        }
        if (iconOrderNumber == 2)
        {
            controller.icon.transform.localPosition = new Vector2(-140, -60);
        }
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
