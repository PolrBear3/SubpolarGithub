using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
        Storage_Text_Connection();
        Storage_Check_for_RefundButton();
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
        controller.mainPanelScript.TurnOff_All_Options();
        ChairBed_options.SetActive(true);
        SpaceTycoon_Main_GameController.isGroundOptionMenuOn = true;
    }
    public void ChairBed_Options_Off()
    {
        controller.mainPanelScript.TurnOff_All_Options();
    }

    // refund
    public GameObject refundButton;
    void Storage_Check_for_RefundButton()
    {
        if (SpaceTycoon_Main_GameController.chairBed_storage == 0)
        {
            refundButton.SetActive(false);
        }
        else
        {
            refundButton.SetActive(true);
        }
    }
    public void Storage_Refund()
    {
        SpaceTycoon_Main_GameController.chairBed_storage -= 1;
        // give back 1/3 of ingredients
    }

    // craft 
    public Text chairBed_storage_text;
    void Storage_Text_Connection()
    {
        chairBed_storage_text.text = "" + SpaceTycoon_Main_GameController.chairBed_storage;
    }

    void Storage_Check()
    {
        if (SpaceTycoon_Main_GameController.chairBed_storage > 0)
        {
            SpaceTycoon_Main_GameController.chairBed_storage -= 1;
        }
        if (SpaceTycoon_Main_GameController.chairBed_storage == 0)
        {
            // ingredients spend
        }
    }


    public void Craft_ChairBed_SnapPoint1()
    {
        Storage_Check();
        Instantiate(chairBed_prefab, controller.GsnapPoint1.transform.position, Quaternion.identity);
    }
    public void Craft_ChairBed_SnapPoint2()
    {
        Storage_Check();
        Instantiate(chairBed_prefab, controller.GsnapPoint2.transform.position, Quaternion.identity);
    }
    public void Craft_ChairBed_SnapPoint3()
    {
        Storage_Check();
        Instantiate(chairBed_prefab, controller.GsnapPoint3.transform.position, Quaternion.identity);
    }
    public void Craft_ChairBed_SnapPoint4()
    {
        Storage_Check();
        Instantiate(chairBed_prefab, controller.GsnapPoint4.transform.position, Quaternion.identity);
    }
    public void Craft_ChairBed_SnapPoint5()
    {
        Storage_Check();
        Instantiate(chairBed_prefab, controller.GsnapPoint5.transform.position, Quaternion.identity);
    }
    public void Craft_ChairBed_SnapPoint6()
    {
        Storage_Check();
        Instantiate(chairBed_prefab, controller.GsnapPoint6.transform.position, Quaternion.identity);
    }
    public void Craft_ChairBed_SnapPoint7()
    {
        Storage_Check();
        Instantiate(chairBed_prefab, controller.GsnapPoint7.transform.position, Quaternion.identity);
    }
}
