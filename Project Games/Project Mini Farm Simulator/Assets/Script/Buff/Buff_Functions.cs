using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Buff_Functions : MonoBehaviour
{
    public Buff_Function_Controller functionController;
    
    public void Cloudy_Stun_Shield(FarmTile farmtile)
    {
        // if the weather is cloudy
        if (functionController.controller.eventSystem.currentWeather.weatherID == 1)
        {
            // give back 1 heatlh

            // get rid of cloudy stun icon

            // remove from buff list
        }
    }
}
