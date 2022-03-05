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
        Automatic_TurnOff();
        Icon_Popup();
    }

    void Default_Position()
    {
        EscapePod_CraftTable_gameObject.transform.position = new Vector2(controller.snapPoint4.transform.position.x, controller.snapPoint4.transform.position.y);
        controller.positionButton4.SetActive(false);
        controller.snapPoint4.SetActive(false);
    }

    // main panel
    public void Manual_TurnOff()
    {
        controller.mainPanel.SetActive(false);
        controller.allSnapPoints.SetActive(false);
    }
    void Automatic_TurnOff()
    {
        if (controller.playerDetection == false)
        {
            controller.mainPanel.SetActive(false);
            controller.allSnapPoints.SetActive(false);
            controller.optionsMenu.SetActive(false);
        }
    }
    public void Icon_Pressed()
    {
        controller.mainPanel.SetActive(true);
        controller.allSnapPoints.SetActive(true);
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
        controller.optionsMenu.SetActive(true);
    }
    public void CraftTable_Options_Off()
    {
        controller.optionsMenu.SetActive(false);
    }

    // craft table change position
    public void SnapPoint1()
    {
        EscapePod_CraftTable_gameObject.transform.position = new Vector2(controller.snapPoint1.transform.position.x, controller.snapPoint1.transform.position.y);
        controller.Restart();
        controller.positionButton1.SetActive(false);
        controller.snapPoint1.SetActive(false);
    }
    public void SnapPoint2()
    {
        EscapePod_CraftTable_gameObject.transform.position = new Vector2(controller.snapPoint2.transform.position.x, controller.snapPoint2.transform.position.y);
        controller.Restart();
        controller.positionButton2.SetActive(false);
        controller.snapPoint2.SetActive(false);
    }
    public void SnapPoint3()
    {
        EscapePod_CraftTable_gameObject.transform.position = new Vector2(controller.snapPoint3.transform.position.x, controller.snapPoint3.transform.position.y);
        controller.Restart();
        controller.positionButton3.SetActive(false);
        controller.snapPoint3.SetActive(false);
    }
    public void SnapPoint4()
    {
        EscapePod_CraftTable_gameObject.transform.position = new Vector2(controller.snapPoint4.transform.position.x, controller.snapPoint4.transform.position.y);
        controller.Restart();
        controller.positionButton4.SetActive(false);
        controller.snapPoint4.SetActive(false);
    }
    public void SnapPoint5()
    {
        EscapePod_CraftTable_gameObject.transform.position = new Vector2(controller.snapPoint5.transform.position.x, controller.snapPoint5.transform.position.y);
        controller.Restart();
        controller.positionButton5.SetActive(false);
        controller.snapPoint5.SetActive(false);
    }
    public void SnapPoint6()
    {
        EscapePod_CraftTable_gameObject.transform.position = new Vector2(controller.snapPoint6.transform.position.x, controller.snapPoint6.transform.position.y);
        controller.Restart();
        controller.positionButton6.SetActive(false);
        controller.snapPoint6.SetActive(false);
    }
    public void SnapPoint7()
    {
        EscapePod_CraftTable_gameObject.transform.position = new Vector2(controller.snapPoint7.transform.position.x, controller.snapPoint7.transform.position.y);
        controller.Restart();
        controller.positionButton7.SetActive(false);
        controller.snapPoint7.SetActive(false);
    }
}
