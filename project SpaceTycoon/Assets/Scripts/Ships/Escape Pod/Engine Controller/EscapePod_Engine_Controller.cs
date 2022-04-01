using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EscapePod_Engine_Controller : MonoBehaviour
{
    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    [HideInInspector]
    public Animator anim;
    
    // main
    [HideInInspector]
    public bool playerDetection;
    public GameObject icon;
    public GameObject iconBoxCollider;
    public GameObject mainPanel;

    // sliders
    public Slider speedSlider;
    [HideInInspector]
    public float currentspeedSliderValue = 0f, setSpeedSliderValue, speedAccelerationValue = 5f;

    public GameObject energyFuel;
    public Slider energyFuelSlider;
    [HideInInspector]
    public float currentEnergyFuel;
    public float maxEnergyFuel = 400f;
    [HideInInspector]
    public bool isEnergyFuelEmpty = false;

    public GameObject emergencyFuel;
    public Slider emergencyEnergyFuelSlider;
    [HideInInspector]
    public float currentEmergencyFuel;
    public float maxEmergencyFuel = 200f;
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
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("player_hand"))
        {
            playerDetection = false;
        }
    }
}
