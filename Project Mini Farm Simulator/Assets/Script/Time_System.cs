using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Time_System : MonoBehaviour
{
    public MainGame_Controller controller;

    private int _currentInGameDay;
    public int currentInGameDay => _currentInGameDay;

    private int maxInGameDay = 365;

    public void Next_Day()
    {
        _currentInGameDay += 1;
        Check_End0f_Year();
        controller.defaultMenu.Update_UI();

        for (int i = 0; i < controller.farmTiles.Length; i++)
        {
            controller.farmTiles[i].Seed_Planted_Status_Update();
        }
    }
    private void Check_End0f_Year()
    {
        if (currentInGameDay == maxInGameDay + 1)
        {
            _currentInGameDay = 0;
        }
    }
}
