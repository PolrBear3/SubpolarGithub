using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EscapePod_Engine_Controller : MonoBehaviour
{
    // main
    [HideInInspector]
    public bool playerDetection;
    public GameObject icon;
    public GameObject iconBoxCollider;
    public GameObject mainPanel;

    // sliders
    public Slider speed;
    public Slider energyFuel;
    public Slider emergencyEnergyFuel;

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
