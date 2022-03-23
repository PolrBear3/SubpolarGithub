using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EscapePod_CraftTable_Panel : MonoBehaviour
{
    public GameObject EscapePod_CraftTable_gameObject;
    public EscapePod_CraftTable_MainController controller;

    void Start()
    {
        Default_Position();
    }

    void Update()
    {
        Automatic_TurnOff_All();
        Icon_Popup();
        Option_Button_Availability();
        OnOff_Ground_SnapPoints();
        OnOff_Wall_SnapPoints();
    }

    private void Option_Button_Availability()
    {
        if (controller.GsnapPoint1.sr.enabled == false)
        {
            controller.positionButton1.SetActive(false);
        }
        else if (controller.GsnapPoint1.sr.enabled == true)
        {
            controller.positionButton1.SetActive(true);
        }

        if (controller.GsnapPoint2.sr.enabled == false)
        {
            controller.positionButton2.SetActive(false);
        }
        else if (controller.GsnapPoint2.sr.enabled == true)
        {
            controller.positionButton2.SetActive(true);
        }

        if (controller.GsnapPoint3.sr.enabled == false)
        {
            controller.positionButton3.SetActive(false);
        }
        else if (controller.GsnapPoint3.sr.enabled == true)
        {
            controller.positionButton3.SetActive(true);
        }

        if (controller.GsnapPoint4.sr.enabled == false)
        {
            controller.positionButton4.SetActive(false);
        }
        else if (controller.GsnapPoint4.sr.enabled == true)
        {
            controller.positionButton4.SetActive(true);
        }

        if (controller.GsnapPoint5.sr.enabled == false)
        {
            controller.positionButton5.SetActive(false);
        }
        else if (controller.GsnapPoint5.sr.enabled == true)
        {
            controller.positionButton5.SetActive(true);
        }

        if (controller.GsnapPoint6.sr.enabled == false)
        {
            controller.positionButton6.SetActive(false);
        }
        else if (controller.GsnapPoint6.sr.enabled == true)
        {
            controller.positionButton6.SetActive(true);
        }

        if (controller.GsnapPoint7.sr.enabled == false)
        {
            controller.positionButton7.SetActive(false);
        }
        else if (controller.GsnapPoint7.sr.enabled == true)
        {
            controller.positionButton7.SetActive(true);
        }
    }

    void Default_Position()
    {
        EscapePod_CraftTable_gameObject.transform.position = new Vector2(controller.GsnapPoint4.transform.position.x, controller.GsnapPoint4.transform.position.y);
    }

    // snap points
    void OnOff_Ground_SnapPoints()
    {
        if (SpaceTycoon_Main_GameController.isGroundOptionMenuOn == true)
        {
            controller.allGroundSnapPoints.SetActive(true);
        }

        if (SpaceTycoon_Main_GameController.isGroundOptionMenuOn == false)
        {
            controller.allGroundSnapPoints.SetActive(false);
        }
    }
    void OnOff_Wall_SnapPoints()
    {
        if (SpaceTycoon_Main_GameController.isWallOptionMenuOn == true)
        {
            controller.allWallSnapPoints.SetActive(true);
        }

        if (SpaceTycoon_Main_GameController.isWallOptionMenuOn == false)
        {
            controller.allWallSnapPoints.SetActive(false);
        }
    }

    // main panel
    public void Manual_TurnOff_All()
    {
        controller.mainPanel.SetActive(false);

        controller.craftTable_options.SetActive(false);
        controller.chairBed_options.SetActive(false);

        SpaceTycoon_Main_GameController.isGroundOptionMenuOn = false;
        SpaceTycoon_Main_GameController.isWallOptionMenuOn = false;
        SpaceTycoon_Main_GameController.isPanelMenuOn = false;
    }
    void Automatic_TurnOff_All()
    {
        if (controller.playerDetection == false && controller.mainPanel.activeSelf == true)
        {
            controller.mainPanel.SetActive(false);

            controller.craftTable_options.SetActive(false);
            controller.chairBed_options.SetActive(false);

            SpaceTycoon_Main_GameController.isGroundOptionMenuOn = false;
            SpaceTycoon_Main_GameController.isWallOptionMenuOn = false;
            SpaceTycoon_Main_GameController.isPanelMenuOn = false;
        }
    }

    // icon
    public void Icon_Pressed()
    {
        controller.mainPanel.SetActive(true);
        SpaceTycoon_Main_GameController.isPanelMenuOn = true;
    }
    void Icon_Popup()
    {
        if (controller.playerDetection == true && SpaceTycoon_Main_GameController.isPanelMenuOn == false)
        {
            controller.icon.SetActive(true);
        }
        if (controller.playerDetection == false || SpaceTycoon_Main_GameController.isPanelMenuOn == true)
        {
            controller.icon.SetActive(false);
        }
    }

    // options menu
    public void CraftTable_Options_On()
    {
        if (SpaceTycoon_Main_GameController.isGroundOptionMenuOn == false)
        {
            controller.craftTable_options.SetActive(true);
            SpaceTycoon_Main_GameController.isGroundOptionMenuOn = true;
        }
    }
    public void CraftTable_Options_Off()
    {
        controller.craftTable_options.SetActive(false);
        SpaceTycoon_Main_GameController.isGroundOptionMenuOn = false;
    }

    // craft table change position
    public void SnapPoint1()
    {
        EscapePod_CraftTable_gameObject.transform.position = new Vector2(controller.GsnapPoint1.transform.position.x, controller.GsnapPoint1.transform.position.y);
    }
    public void SnapPoint2()
    {
        EscapePod_CraftTable_gameObject.transform.position = new Vector2(controller.GsnapPoint2.transform.position.x, controller.GsnapPoint2.transform.position.y);
    }
    public void SnapPoint3()
    {
        EscapePod_CraftTable_gameObject.transform.position = new Vector2(controller.GsnapPoint3.transform.position.x, controller.GsnapPoint3.transform.position.y);
    }
    public void SnapPoint4()
    {
        EscapePod_CraftTable_gameObject.transform.position = new Vector2(controller.GsnapPoint4.transform.position.x, controller.GsnapPoint4.transform.position.y);
    }
    public void SnapPoint5()
    {
        EscapePod_CraftTable_gameObject.transform.position = new Vector2(controller.GsnapPoint5.transform.position.x, controller.GsnapPoint5.transform.position.y);
    }
    public void SnapPoint6()
    {
        EscapePod_CraftTable_gameObject.transform.position = new Vector2(controller.GsnapPoint6.transform.position.x, controller.GsnapPoint6.transform.position.y);
    }
    public void SnapPoint7()
    {
        EscapePod_CraftTable_gameObject.transform.position = new Vector2(controller.GsnapPoint7.transform.position.x, controller.GsnapPoint7.transform.position.y);
    }
}