using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EscapePod_Engine_Controller : SpaceTycoon_Main_GameController
{
    [HideInInspector]
    public Animator anim;
    
    // main
    [HideInInspector]
    public bool playerDetection;
    public GameObject Icon, iconBoxCollider, mainPanel;
    public Icon icon;

    // sliders
    public Slider speedSlider;
    [HideInInspector]
    public float currentspeedSliderValue = 0f, setSpeedSliderValue, speedAccelerationValue = 5f;

    // energy fuel
    public GameObject energyFuel;
    public Slider energyFuelSlider;
    public float maxEnergyFuel = 400f;
    [HideInInspector]
    public float currentEnergyFuel;
    [HideInInspector]
    public bool EnergyFuelDepleted = false;

    // emergency energy fuel
    public GameObject emergencyFuel;
    public Slider emergencyEnergyFuelSlider;
    public float maxEmergencyFuel = 200f;
    [HideInInspector]
    public float currentEmergencyFuel;
    [HideInInspector]
    public bool EmergencyFuelDepleted = false;

    // engine on off buttons and lights
    public GameObject mainEngineOnButton, mainEngineOffButton, mainEngineLight;
    public GameObject side1EngineOnButton, side1EngineOffButton, side1EngineLight;
    public GameObject side2EngineOnButton, side2EngineOffButton, side2EngineLight;
    
    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    private void Start()
    {
        Set_Max_Min_EnergyFuel_atStart();
    }

    private void Update()
    {
        Icon_Popup_UpdateCheck(playerDetection, Icon);
        Automatic_TurnOff_ObjectPanel(playerDetection, mainPanel);
        Ship_Speed_Condition();
        EnergyFuel_Condition();
        EmergencyFuel_Condition();
        EnergyFuel_Depletion_Check();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("player_hand"))
        {
            playerDetection = true;
            icon.Set_Icon_Position();
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("player_hand"))
        {
            playerDetection = false;
            icon.Set_Icon_to_Default_Position();
        }
    }

    // Engine On and Off
    public void MainEngine_On_Press()
    {
        Engine_On_Pressed(mainEngineOnButton, mainEngineOffButton, mainEngineLight);
    }
    public void MainEngine_Off_Press()
    {
        Engine_Off_Pressed(mainEngineOnButton, mainEngineOffButton, mainEngineLight);
    }

    public void Side1Engine_On_Press()
    {
        Engine_On_Pressed(side1EngineOnButton, side1EngineOffButton, side1EngineLight);
    }
    public void Side1Engine_Off_Press()
    {
        Engine_Off_Pressed(side1EngineOnButton, side1EngineOffButton, side1EngineLight);
    } 

    public void Side2Engine_On_Press()
    {
        Engine_On_Pressed(side2EngineOnButton, side2EngineOffButton, side2EngineLight);
    }
    public void Side2Engine_Off_Press()
    {
        Engine_Off_Pressed(side2EngineOnButton, side2EngineOffButton, side2EngineLight);
    }

    // Energy Fuel Conditions
    void Set_Max_Min_EnergyFuel_atStart()
    {
        currentEnergyFuel = maxEnergyFuel;
        currentEmergencyFuel = maxEmergencyFuel;
    }

    // Speed and EnergyFuel Conditions
    void Ship_Speed_Condition()
    {
        // UI Slider Bar
        speedSlider.value = currentspeedSliderValue;

        if (EnginesOn == 0)
        {
            setSpeedSliderValue = 0f;
        }
        if (EnginesOn == 1)
        {
            setSpeedSliderValue = 30f;
        }
        if (EnginesOn == 2)
        {
            setSpeedSliderValue = 60f;
        }
        if (EnginesOn == 3)
        {
            setSpeedSliderValue = 90f;
        }

        // gradual slider decrease
        if (currentspeedSliderValue > setSpeedSliderValue)
        {
            currentspeedSliderValue = currentspeedSliderValue - (speedAccelerationValue * Time.deltaTime);
        }
        // gradual slider increase
        else if (currentspeedSliderValue < setSpeedSliderValue)
        {
            currentspeedSliderValue = currentspeedSliderValue + (speedAccelerationValue * Time.deltaTime);
        }
    }
    void EnergyFuel_Condition()
    {
        // UI Slider Bar
        energyFuelSlider.value = currentEnergyFuel;

        if (EnergyFuelDepleted == false)
        {
            if (EnginesOn == 1)
            {
                currentEnergyFuel -= 1 * Time.deltaTime;
            }
            if (EnginesOn == 2)
            {
                currentEnergyFuel -= 2 * Time.deltaTime;
            }
            if (EnginesOn == 3)
            {
                currentEnergyFuel -= 3 * Time.deltaTime;
            }
        }
    }
    void EmergencyFuel_Condition()
    {
        // UI Slider Bar
        emergencyEnergyFuelSlider.value = currentEmergencyFuel;

        if (EnergyFuelDepleted && EmergencyFuelDepleted == false)
        {
            if (EnginesOn == 1)
            {
                currentEmergencyFuel -= 1 * Time.deltaTime;
            }
            if (EnginesOn == 2)
            {
                currentEmergencyFuel -= 2 * Time.deltaTime;
            }
            if (EnginesOn == 3)
            {
                currentEmergencyFuel -= 3 * Time.deltaTime;
            }
        }
    }

    void EnergyFuel_Depletion_Check()
    {
        if (currentEnergyFuel <= 0)
        {
            EnergyFuelDepleted = true;
            emergencyFuel.SetActive(true);
            energyFuel.SetActive(false);
            anim.SetBool("isUsingEmergencyFuel", true);
        }
        if (currentEnergyFuel > 0)
        {
            EnergyFuelDepleted = false;
            energyFuel.SetActive(true);
            emergencyFuel.SetActive(false);
            anim.SetBool("isUsingEmergencyFuel", false);
        }

        if (currentEmergencyFuel <= 0)
        {
            EmergencyFuelDepleted = true;
        }
        if (currentEmergencyFuel > 0)
        {
            EmergencyFuelDepleted = false;
        }

        if (EnergyFuelDepleted && EmergencyFuelDepleted)
        {
            MainEngine_Off_Press();
            Side1Engine_Off_Press();
            Side2Engine_Off_Press();
        }
    }
}