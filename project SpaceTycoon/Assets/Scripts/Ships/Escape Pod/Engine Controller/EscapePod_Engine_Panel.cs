using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EscapePod_Engine_Panel : MonoBehaviour
{
    public EscapePod_Engine_Controller controller;

    private void Start()
    {
        Set_Max_Min_EnergyFuel();
    }

    private void Update()
    {
        Icon_Popup();
        Automatic_Off();
        Speed_SliderSet();
        EnergyFuel_Empty_Check();
        EnergyFuel_SliderSet();
        EmergencyFuel_SliderSet();
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

    // speed
    void Speed_SliderSet()
    {
        controller.speedSlider.value = controller.currentspeedSliderValue;

        if (SpaceTycoon_Main_GameController.EnginesOn == 0)
        {
            controller.setSpeedSliderValue = 0f;
        }
        if (SpaceTycoon_Main_GameController.EnginesOn == 1)
        {
            controller.setSpeedSliderValue = 30f;
        }
        if (SpaceTycoon_Main_GameController.EnginesOn == 2)
        {
            controller.setSpeedSliderValue = 60f;
        }
        if (SpaceTycoon_Main_GameController.EnginesOn == 3)
        {
            controller.setSpeedSliderValue = 90f;
        }

        if (controller.currentspeedSliderValue > controller.setSpeedSliderValue)
        {
            controller.currentspeedSliderValue = controller.currentspeedSliderValue - (controller.speedAccelerationValue * Time.deltaTime);
        }
        else if (controller.currentspeedSliderValue < controller.setSpeedSliderValue)
        {
            controller.currentspeedSliderValue = controller.currentspeedSliderValue + (controller.speedAccelerationValue * Time.deltaTime);
        }
    }
    
    // fuel
    void Set_Max_Min_EnergyFuel()
    {
        controller.currentEnergyFuel = controller.maxEnergyFuel;
        controller.currentEmergencyFuel = controller.maxEmergencyFuel;
    }
    void EnergyFuel_Empty_Check()
    {
        if (controller.currentEnergyFuel <= 0)
        {
            controller.isEnergyFuelEmpty = true;
            controller.emergencyFuel.SetActive(true);
            controller.energyFuel.SetActive(false);
            controller.anim.SetBool("isUsingEmergencyFuel", true);
        }
        if (controller.currentEnergyFuel > 0)
        {
            controller.isEnergyFuelEmpty = false;
            controller.energyFuel.SetActive(true);
            controller.emergencyFuel.SetActive(false);
            controller.anim.SetBool("isUsingEmergencyFuel", false);
        }

        if (controller.currentEmergencyFuel <= 0)
        {
            controller.isEmergencyFuelEmpty = true;
        }
        if (controller.currentEmergencyFuel > 0)
        {
            controller.isEmergencyFuelEmpty = false;
        }

        if (controller.isEnergyFuelEmpty && controller.isEmergencyFuelEmpty)
        {
            All_Engines_Off();
        }
    }

    void EnergyFuel_SliderSet()
    {
        controller.energyFuelSlider.value = controller.currentEnergyFuel;

        if (controller.isEnergyFuelEmpty == false)
        {
            if (SpaceTycoon_Main_GameController.EnginesOn == 1)
            {
                controller.currentEnergyFuel -= 1 * Time.deltaTime;
            }
            if (SpaceTycoon_Main_GameController.EnginesOn == 2)
            {
                controller.currentEnergyFuel -= 2 * Time.deltaTime;
            }
            if (SpaceTycoon_Main_GameController.EnginesOn == 3)
            {
                controller.currentEnergyFuel -= 3 * Time.deltaTime;
            }
        }
    }
    void EmergencyFuel_SliderSet()
    {
        controller.emergencyEnergyFuelSlider.value = controller.currentEmergencyFuel;

        if (controller.isEnergyFuelEmpty && controller.isEmergencyFuelEmpty == false)
        {
            if (SpaceTycoon_Main_GameController.EnginesOn == 1)
            {
                controller.currentEmergencyFuel -= 1 * Time.deltaTime;
            }
            if (SpaceTycoon_Main_GameController.EnginesOn == 2)
            {
                controller.currentEmergencyFuel -= 2 * Time.deltaTime;
            }
            if (SpaceTycoon_Main_GameController.EnginesOn == 3)
            {
                controller.currentEmergencyFuel -= 3 * Time.deltaTime;
            }
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

    void All_Engines_Off()
    {
        MainEngineOff();
        Side1EngineOff();
        Side2EngineOff();
    }
}
