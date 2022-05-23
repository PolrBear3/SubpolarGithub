using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EscapePod_Engine_Controller : SpaceTycoon_Main_GameController
{
    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        Icon_Popup_UpdateCheck(playerDetection, Icon);
    }

    public void Icon_Press()
    {
        Icon_Pressed(Icon);
    }

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
    [HideInInspector]
    public float maxEnergyFuel = 400f;
    public float currentEnergyFuel;
    [HideInInspector]
    public bool isEnergyFuelEmpty = false;

    // emergency energy fuel
    public GameObject emergencyFuel;
    public Slider emergencyEnergyFuelSlider;
    public float maxEmergencyFuel = 200f;
    [HideInInspector]
    public float currentEmergencyFuel;
    [HideInInspector]
    public bool isEmergencyFuelEmpty = false;

    // engine on off buttons
    public GameObject mainEngineOnButton, mainEngineOffButton, mainEngineLight;
    public GameObject side1EngineOnButton, side1EngineOffButton, side1EngineLight;
    public GameObject side2EngineOnButton, side2EngineOffButton, side2EngineLight;
    
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
}
