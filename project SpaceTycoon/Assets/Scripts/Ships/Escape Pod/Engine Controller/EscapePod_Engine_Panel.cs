using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EscapePod_Engine_Panel : MonoBehaviour
{
    public EscapePod_Engine_Controller controller;
    
    private void Update()
    {
        Icon_Popup();
        Automatic_Off();
        speedSliderSet();
    }

    // icon
    void Icon_Popup()
    {
        // on
        if (controller.playerDetection == true && SpaceTycoon_Main_GameController.isPanelMenuOn == false)
        {
            controller.icon.SetActive(true);
        }
        // off
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

    // sliders
    void speedSliderSet()
    {
        controller.speed.value = controller.currentspeedSliderValue;

        if (SpaceTycoon_Main_GameController.shipSpeed0)
        {
            controller.setSpeedSliderValue = 0f;
        }
        if (SpaceTycoon_Main_GameController.shipSpeed1)
        {
            controller.setSpeedSliderValue = 30f;
        }
        if (SpaceTycoon_Main_GameController.shipSpeed2)
        {
            controller.setSpeedSliderValue = 60f;
        }
        if (SpaceTycoon_Main_GameController.shipSpeed3)
        {
            controller.setSpeedSliderValue = 90f;
        }

        if (controller.currentspeedSliderValue > controller.setSpeedSliderValue)
        {
            controller.currentspeedSliderValue = controller.currentspeedSliderValue - (controller.accelerationValue * Time.deltaTime);
        }
        else if (controller.currentspeedSliderValue < controller.setSpeedSliderValue)
        {
            controller.currentspeedSliderValue = controller.currentspeedSliderValue + (controller.accelerationValue * Time.deltaTime);
        }
    }

    // engine On Off
    public void MainEngineOn()
    {
        controller.mainEngineOnButton.SetActive(false);
        controller.mainEngineOffButton.SetActive(true);
        controller.mainEngineLight.SetActive(true);
        SpaceTycoon_Main_GameController.EnginesOn += 1;
    }
    public void MainEngineOff()
    {
        controller.mainEngineOnButton.SetActive(true);
        controller.mainEngineOffButton.SetActive(false);
        controller.mainEngineLight.SetActive(false);
        SpaceTycoon_Main_GameController.EnginesOn -= 1;
    }

    public void Side1EngineOn()
    {
        controller.side1EngineOnButton.SetActive(false);
        controller.side1EngineOffButton.SetActive(true);
        controller.side1EngineLight.SetActive(true);
        SpaceTycoon_Main_GameController.EnginesOn += 1;
    }
    public void Side1EngineOff()
    {
        controller.side1EngineOnButton.SetActive(true);
        controller.side1EngineOffButton.SetActive(false);
        controller.side1EngineLight.SetActive(false);
        SpaceTycoon_Main_GameController.EnginesOn -= 1;
    }

    public void Side2EngineOn()
    {
        controller.side2EngineOnButton.SetActive(false);
        controller.side2EngineOffButton.SetActive(true);
        controller.side2EngineLight.SetActive(true);
        SpaceTycoon_Main_GameController.EnginesOn += 1;
    }
    public void Side2EngineOff()
    {
        controller.side2EngineOnButton.SetActive(true);
        controller.side2EngineOffButton.SetActive(false);
        controller.side2EngineLight.SetActive(false);
        SpaceTycoon_Main_GameController.EnginesOn -= 1;
    }
}
