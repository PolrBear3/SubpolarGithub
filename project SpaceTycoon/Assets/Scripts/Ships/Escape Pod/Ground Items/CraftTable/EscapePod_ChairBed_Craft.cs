using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EscapePod_ChairBed_Craft : MonoBehaviour
{
    public EscapePod_CraftTable_MainController controller;

    public GameObject chairBed_prefab;
    
    public GameObject ChairBed_options;

    public GameObject positionButton1, positionButton2, positionButton3, 
                      positionButton4, positionButton5, positionButton6, positionButton7;

    private void Update()
    {
        Option_Button_Availability();
    }

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
    public void ChairBed_Options_On()
    {
        if (SpaceTycoon_Main_GameController.isGroundOptionMenuOn == false)
        {
            ChairBed_options.SetActive(true);
            SpaceTycoon_Main_GameController.isGroundOptionMenuOn = true;
        }
    }
    public void ChairBed_Options_Off()
    {
        ChairBed_options.SetActive(false);
        SpaceTycoon_Main_GameController.isGroundOptionMenuOn = false;
    }

    // craft chairBed
    public void Craft_ChairBed_SnapPoint1()
    {
        Instantiate(chairBed_prefab, controller.GsnapPoint1.transform.position, Quaternion.identity);
    }
    public void Craft_ChairBed_SnapPoint2()
    {
        Instantiate(chairBed_prefab, controller.GsnapPoint2.transform.position, Quaternion.identity);
    }
    public void Craft_ChairBed_SnapPoint3()
    {
        Instantiate(chairBed_prefab, controller.GsnapPoint3.transform.position, Quaternion.identity);
    }
    public void Craft_ChairBed_SnapPoint4()
    {
        Instantiate(chairBed_prefab, controller.GsnapPoint4.transform.position, Quaternion.identity);
    }
    public void Craft_ChairBed_SnapPoint5()
    {
        Instantiate(chairBed_prefab, controller.GsnapPoint5.transform.position, Quaternion.identity);
    }
    public void Craft_ChairBed_SnapPoint6()
    {
        Instantiate(chairBed_prefab, controller.GsnapPoint6.transform.position, Quaternion.identity);
    }
    public void Craft_ChairBed_SnapPoint7()
    {
        Instantiate(chairBed_prefab, controller.GsnapPoint7.transform.position, Quaternion.identity);
    }
}
