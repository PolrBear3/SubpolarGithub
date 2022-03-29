using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EscapePod_Closet_Craft : MonoBehaviour
{
    private void Update()
    {
        Option_Button_Availability();
    }

    public EscapePod_CraftTable_MainController controller;

    public GameObject closetPrefab, closetOption;

    public GameObject positionButton1, positionButton2, positionButton3,
                      positionButton4, positionButton5, positionButton6, positionButton7;

    private void Option_Button_Availability()
    {
        if (controller.GsnapPoint1.sr.enabled == false)
        {
            positionButton1.SetActive(false);
        }
        else if (controller.GsnapPoint1.sr.enabled == true)
        {
            positionButton1.SetActive(true);
        }

        if (controller.GsnapPoint2.sr.enabled == false)
        {
            positionButton2.SetActive(false);
        }
        else if (controller.GsnapPoint2.sr.enabled == true)
        {
            positionButton2.SetActive(true);
        }

        if (controller.GsnapPoint3.sr.enabled == false)
        {
            positionButton3.SetActive(false);
        }
        else if (controller.GsnapPoint3.sr.enabled == true)
        {
            positionButton3.SetActive(true);
        }

        if (controller.GsnapPoint4.sr.enabled == false)
        {
            positionButton4.SetActive(false);
        }
        else if (controller.GsnapPoint4.sr.enabled == true)
        {
            positionButton4.SetActive(true);
        }

        if (controller.GsnapPoint5.sr.enabled == false)
        {
            positionButton5.SetActive(false);
        }
        else if (controller.GsnapPoint5.sr.enabled == true)
        {
            positionButton5.SetActive(true);
        }

        if (controller.GsnapPoint6.sr.enabled == false)
        {
            positionButton6.SetActive(false);
        }
        else if (controller.GsnapPoint6.sr.enabled == true)
        {
            positionButton6.SetActive(true);
        }

        if (controller.GsnapPoint7.sr.enabled == false)
        {
            positionButton7.SetActive(false);
        }
        else if (controller.GsnapPoint7.sr.enabled == true)
        {
            positionButton7.SetActive(true);
        }
    }

    // options
    public void Closet_Option_On()
    {
        controller.mainPanelScript.TurnOff_All_Options();
        closetOption.SetActive(true);
        SpaceTycoon_Main_GameController.isGroundOptionMenuOn = true;
    }
    public void Closet_Option_Off()
    {
        controller.mainPanelScript.TurnOff_All_Options();
    }

    // craft
    public void Craft_SnapPoint1()
    {
        Instantiate(closetPrefab, controller.GsnapPoint1.transform.position, Quaternion.identity);
    }
    public void Craft_SnapPoint2()
    {
        Instantiate(closetPrefab, controller.GsnapPoint2.transform.position, Quaternion.identity);
    }
    public void Craft_SnapPoint3()
    {
        Instantiate(closetPrefab, controller.GsnapPoint3.transform.position, Quaternion.identity);
    }
    public void Craft_SnapPoint4()
    {
        Instantiate(closetPrefab, controller.GsnapPoint4.transform.position, Quaternion.identity);
    }
    public void Craft_SnapPoint5()
    {
        Instantiate(closetPrefab, controller.GsnapPoint5.transform.position, Quaternion.identity);
    }
    public void Craft_SnapPoint6()
    {
        Instantiate(closetPrefab, controller.GsnapPoint6.transform.position, Quaternion.identity);
    }
    public void Craft_SnapPoint7()
    {
        Instantiate(closetPrefab, controller.GsnapPoint7.transform.position, Quaternion.identity);
    }
}
