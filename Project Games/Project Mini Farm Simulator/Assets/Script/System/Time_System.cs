using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Time_System : MonoBehaviour
{
    public MainGame_Controller controller;

    public Season_ScrObj[] allSeasons;

    public Season_ScrObj currentSeason;

    private int _currentInGameDay = 0;
    public int currentInGameDay => _currentInGameDay;

    private int maxInGameDay = 365;

    private void Start()
    {
        Next_Day();
    }

    public void Next_Day()
    {
        _currentInGameDay += 1;

        Check_End0f_Year();
        Check_Season();

        controller.eventSystem.Set_Today_Weather();
        controller.defaultMenu.Update_UI();

        for (int i = 0; i < controller.farmTiles.Length; i++)
        {
            controller.farmTiles[i].Seed_Planted_Status_Update();
        }
    }
    private void Check_Season()
    {
        // spring
        if (currentInGameDay >= 1 && currentInGameDay <= 93) { currentSeason = allSeasons[0]; }
        // summer
        else if (currentInGameDay >= 94 && currentInGameDay <= 186) { currentSeason = allSeasons[1]; }
        // fall
        else if (currentInGameDay >= 187 && currentInGameDay <= 279) { currentSeason = allSeasons[2]; }
        // winter
        else if (currentInGameDay >= 280 && currentInGameDay <= 365) { currentSeason = allSeasons[3]; }
    }
    private void Check_End0f_Year()
    {
        if (currentInGameDay == maxInGameDay + 1)
        {
            _currentInGameDay = 1;
        }
    }
}
