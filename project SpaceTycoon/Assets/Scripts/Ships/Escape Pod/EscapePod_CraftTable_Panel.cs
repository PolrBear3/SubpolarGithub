using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
        OnOff_SnapPoints();
    }

    void Default_Position()
    {
        EscapePod_CraftTable_gameObject.transform.position = new Vector2(controller.snapPoint4.transform.position.x, controller.snapPoint4.transform.position.y);
    }

    void OnOff_SnapPoints()
    {
        if (controller.playerDetection == true)
        {
            controller.snapPoint1.sr.enabled = true;
            controller.snapPoint2.sr.enabled = true;
            controller.snapPoint3.sr.enabled = true;
            controller.snapPoint4.sr.enabled = true;
            controller.snapPoint5.sr.enabled = true;
            controller.snapPoint6.sr.enabled = true;
            controller.snapPoint7.sr.enabled = true;
        }

        if (controller.playerDetection == false)
        {
            controller.snapPoint1.sr.enabled = false;
            controller.snapPoint2.sr.enabled = false;
            controller.snapPoint3.sr.enabled = false;
            controller.snapPoint4.sr.enabled = false;
            controller.snapPoint5.sr.enabled = false;
            controller.snapPoint6.sr.enabled = false;
            controller.snapPoint7.sr.enabled = false;
        }
    }

    private void Option_Button_Availability()
    {
        if (controller.snapPoint1.sr.enabled == false)
        {
            controller.positionButton1.SetActive(false);
        }
        else if (controller.snapPoint1.sr.enabled == true)
        {
            controller.positionButton1.SetActive(true);
        }

        if (controller.snapPoint2.sr.enabled == false)
        {
            controller.positionButton2.SetActive(false);
        }
        else if (controller.snapPoint2.sr.enabled == true)
        {
            controller.positionButton2.SetActive(true);
        }

        if (controller.snapPoint3.sr.enabled == false)
        {
            controller.positionButton3.SetActive(false);
        }
        else if (controller.snapPoint3.sr.enabled == true)
        {
            controller.positionButton3.SetActive(true);
        }

        if (controller.snapPoint4.sr.enabled == false)
        {
            controller.positionButton4.SetActive(false);
        }
        else if (controller.snapPoint4.sr.enabled == true)
        {
            controller.positionButton4.SetActive(true);
        }

        if (controller.snapPoint5.sr.enabled == false)
        {
            controller.positionButton5.SetActive(false);
        }
        else if (controller.snapPoint5.sr.enabled == true)
        {
            controller.positionButton5.SetActive(true);
        }

        if (controller.snapPoint6.sr.enabled == false)
        {
            controller.positionButton6.SetActive(false);
        }
        else if (controller.snapPoint6.sr.enabled == true)
        {
            controller.positionButton6.SetActive(true);
        }

        if (controller.snapPoint7.sr.enabled == false)
        {
            controller.positionButton7.SetActive(false);
        }
        else if (controller.snapPoint7.sr.enabled == true)
        {
            controller.positionButton7.SetActive(true);
        }
    }

    // main panel
    public void Manual_TurnOff_All()
    {
        controller.mainPanel.SetActive(false);
        controller.craftTable_options.SetActive(false);
        SpaceTycoon_Main_GameController.isOptionMenuOn = false;
    }
    void Automatic_TurnOff_All()
    {
        if (controller.playerDetection == false)
        {
            controller.mainPanel.SetActive(false);
            controller.craftTable_options.SetActive(false);
            SpaceTycoon_Main_GameController.isOptionMenuOn = false;
        }
    }
    public void Icon_Pressed()
    {
        controller.mainPanel.SetActive(true);
    }
    void Icon_Popup()
    {
        if (controller.playerDetection == true)
        {
            controller.icon.SetActive(true);
        }
        else if (controller.playerDetection == false)
        {
            controller.icon.SetActive(false);
        }

        if (controller.mainPanel.activeSelf == true)
        {
            controller.icon.SetActive(false);
        }
    }

    // craft table options menu
    public void CraftTable_Options_On()
    {
        if (SpaceTycoon_Main_GameController.isOptionMenuOn == false)
        {
            controller.craftTable_options.SetActive(true);
            SpaceTycoon_Main_GameController.isOptionMenuOn = true;
        }
    }
    public void CraftTable_Options_Off()
    {
        controller.craftTable_options.SetActive(false);
        SpaceTycoon_Main_GameController.isOptionMenuOn = false;
    }

    // craft table change position
    public void SnapPoint1()
    {
        EscapePod_CraftTable_gameObject.transform.position = new Vector2(controller.snapPoint1.transform.position.x, controller.snapPoint1.transform.position.y);
    }
    public void SnapPoint2()
    {
        EscapePod_CraftTable_gameObject.transform.position = new Vector2(controller.snapPoint2.transform.position.x, controller.snapPoint2.transform.position.y);
    }
    public void SnapPoint3()
    {
        EscapePod_CraftTable_gameObject.transform.position = new Vector2(controller.snapPoint3.transform.position.x, controller.snapPoint3.transform.position.y);
    }
    public void SnapPoint4()
    {
        EscapePod_CraftTable_gameObject.transform.position = new Vector2(controller.snapPoint4.transform.position.x, controller.snapPoint4.transform.position.y);
    }
    public void SnapPoint5()
    {
        EscapePod_CraftTable_gameObject.transform.position = new Vector2(controller.snapPoint5.transform.position.x, controller.snapPoint5.transform.position.y);
    }
    public void SnapPoint6()
    {
        EscapePod_CraftTable_gameObject.transform.position = new Vector2(controller.snapPoint6.transform.position.x, controller.snapPoint6.transform.position.y);
    }
    public void SnapPoint7()
    {
        EscapePod_CraftTable_gameObject.transform.position = new Vector2(controller.snapPoint7.transform.position.x, controller.snapPoint7.transform.position.y);
    }
}