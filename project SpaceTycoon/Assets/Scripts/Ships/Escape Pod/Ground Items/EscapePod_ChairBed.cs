using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EscapePod_ChairBed : MonoBehaviour
{
    public EscapePod_CraftTable_MainController controller;
    
    public GameObject ChairBed_options;

    public snapPoint snapPoint1, snapPoint2, snapPoint3, 
                      snapPoint4, snapPoint5, snapPoint6, snapPoint7;

    public GameObject positionButton1, positionButton2, positionButton3, 
                      positionButton4, positionButton5, positionButton6, positionButton7;

    private void Update()
    {
        Option_Button_Availability();
    }

    public void ChairBed_Options_On()
    {
        if (SpaceTycoon_Main_GameController.isOptionMenuOn == false)
        {
            ChairBed_options.SetActive(true);
            SpaceTycoon_Main_GameController.isOptionMenuOn = true;
        }
    }
    public void ChairBed_Options_Off()
    {
        ChairBed_options.SetActive(false);
        SpaceTycoon_Main_GameController.isOptionMenuOn = false;
    }

    private void Option_Button_Availability()
    {
        if (controller.snapPoint1.sr.enabled == false)
        {
            positionButton1.SetActive(false);
        }
        else if (controller.snapPoint1.sr.enabled == true)
        {
            positionButton1.SetActive(true);
        }

        if (controller.snapPoint2.sr.enabled == false)
        {
            positionButton2.SetActive(false);
        }
        else if (controller.snapPoint2.sr.enabled == true)
        {
            positionButton2.SetActive(true);
        }

        if (controller.snapPoint3.sr.enabled == false)
        {
            positionButton3.SetActive(false);
        }
        else if (controller.snapPoint3.sr.enabled == true)
        {
            positionButton3.SetActive(true);
        }

        if (controller.snapPoint4.sr.enabled == false)
        {
            positionButton4.SetActive(false);
        }
        else if (controller.snapPoint4.sr.enabled == true)
        {
            positionButton4.SetActive(true);
        }

        if (controller.snapPoint5.sr.enabled == false)
        {
            positionButton5.SetActive(false);
        }
        else if (controller.snapPoint5.sr.enabled == true)
        {
            positionButton5.SetActive(true);
        }

        if (controller.snapPoint6.sr.enabled == false)
        {
            positionButton6.SetActive(false);
        }
        else if (controller.snapPoint6.sr.enabled == true)
        {
            positionButton6.SetActive(true);
        }

        if (controller.snapPoint7.sr.enabled == false)
        {
            positionButton7.SetActive(false);
        }
        else if (controller.snapPoint7.sr.enabled == true)
        {
            positionButton7.SetActive(true);
        }
    }

    public void Craft_ChairBed_SnapPoint1()
    {

    }
    public void Craft_ChairBed_SnapPoint2()
    {

    }
    public void Craft_ChairBed_SnapPoint3()
    {

    }
    public void Craft_ChairBed_SnapPoint4()
    {

    }
    public void Craft_ChairBed_SnapPoint5()
    {

    }
    public void Craft_ChairBed_SnapPoint6()
    {

    }
    public void Craft_ChairBed_SnapPoint7()
    {

    }
}
