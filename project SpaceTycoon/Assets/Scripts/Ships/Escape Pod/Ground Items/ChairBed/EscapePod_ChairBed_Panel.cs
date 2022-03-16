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
    public void Rotate_Right()
    {
        facingLeft = !facingLeft;
        EscapePod_ChairBed_gameObject.transform.Rotate(0f, 180f, 0f);
        controller.rotateRightButton.SetActive(false);
        controller.roateLeftButton.SetActive(true);
    }
    public void Rotate_Left()
    {
        facingLeft = !facingLeft;
        EscapePod_ChairBed_gameObject.transform.Rotate(0f, 180f, 0f);
        controller.roateLeftButton.SetActive(false);
        controller.rotateRightButton.SetActive(true);
    }

    public void Dismantle()
    {
        controller.iconBoxCollider.SetActive(false);
        Destroy(EscapePod_ChairBed_gameObject);
        // give back chairBed ingredients to player
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
