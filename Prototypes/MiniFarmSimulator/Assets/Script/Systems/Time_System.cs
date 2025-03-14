using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Time_System : MonoBehaviour
{
    public MainGame_Controller controller;

    public Season_ScrObj currentSeason;
    public int currentInGameDay = 0;
    private int maxInGameDay = 365;

    private float pastSec, futureSec;
    [HideInInspector]
    public float mySec;

    private void Start()
    {
        if (!ES3.KeyExists("gameSaved"))
        {
            New_Game();
            Debug.Log("New Game");
        }
        else
        {
            Load_MyTime();
            Set_FutureTime();
            SubtractTime_SinceExit();
        }
    }
    private void Update()
    {
        Run_Time();
        MyTime_Text_Update();
        NextDay_Button_Availability();
    }
    private void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            Save_MyTime();
            Set_PastTime();
        }
        else if (!pause)
        {
            Load_MyTime();
            Set_FutureTime();
            SubtractTime_SinceExit();
        }
    }
    private void OnApplicationQuit()
    {
        Save_MyTime();
        Set_PastTime();
    }

    // in game system
    public void Check_End0f_Year()
    {
        if (currentInGameDay == maxInGameDay + 1)
        {
            currentInGameDay = 1;
        }
    }
    public void Check_Season()
    {
        // spring
        if (currentInGameDay >= 1 && currentInGameDay <= 93) { currentSeason = controller.allSeasons[0]; }
        // summer
        else if (currentInGameDay >= 94 && currentInGameDay <= 186) { currentSeason = controller.allSeasons[1]; }
        // fall
        else if (currentInGameDay >= 187 && currentInGameDay <= 279) { currentSeason = controller.allSeasons[2]; }
        // winter
        else if (currentInGameDay >= 280 && currentInGameDay <= 365) { currentSeason = controller.allSeasons[3]; }
    }

    private void New_Game()
    {
        currentInGameDay += 1;

        Check_End0f_Year();
        Check_Season();

        controller.eventSystem.weatherSystem.StartSet_Estimate_Weathers();
        controller.weatherNewsMenu.Set_NewsIcon_Blink(false);
        controller.eventSystem.Set_Today_Weather();

        // sound
        controller.soundController.Play_SFX(controller.eventSystem.data.currentWeather.weatherSFX);
        controller.soundController.Play_BGM(controller.eventSystem.data.currentWeather.weatherBGM);

        controller.defaultMenu.Update_UI();
        controller.defaultMenu.Next_Day_Animation();

        // +1 day, watered false
        controller.All_FarmTile_NextDay_Update();
        // reset all status
        controller.All_FarmTile_Reset_Status();
        // reset all events
        controller.eventSystem.Reset_All_Events();
        // activate all events
        controller.eventSystem.Activate_All_Events();
        // reset all buffs
        controller.buffSystem.Reset_All_Buffs();
        // activate all buffs
        controller.buffSystem.Activate_All_Buffs();
        // watering check
        controller.All_FarmTile_WateringCheck();
        // health check
        controller.All_FarmTile_HealthCheck();
        // update sprite
        controller.All_FarmTile_Progress_Update();

        ReCalculate_AllSeed_WaitTime();
    }
    public void Next_Day()
    {
        controller.defaultMenu.Next_Day_FadeIn_Previous_Data();

        currentInGameDay += 1;

        Check_End0f_Year();
        Check_Season();

        controller.eventSystem.weatherSystem.Next_Estimate_Weathers();
        controller.weatherNewsMenu.Set_NewsIcon_Blink(false);
        controller.eventSystem.Set_Today_Weather();

        // sound
        controller.soundController.Play_SFX(controller.eventSystem.data.currentWeather.weatherSFX);
        controller.soundController.Play_BGM(controller.eventSystem.data.currentWeather.weatherBGM);

        controller.defaultMenu.Update_UI();
        controller.defaultMenu.Next_Day_Animation();

        // +1 day, watered false
        controller.All_FarmTile_NextDay_Update();
        // reset all status
        controller.All_FarmTile_Reset_Status();
        // reset all events
        controller.eventSystem.Reset_All_Events();
        // activate all events
        controller.eventSystem.Activate_All_Events();
        // reset all buffs
        controller.buffSystem.Reset_All_Buffs();
        // activate all buffs
        controller.buffSystem.Activate_All_Buffs();

        // tutorial guide check for farmtile
        if (controller.tutorial != null)
        {
            controller.tutorial.Tutorial_FarmTile_Update();
        }

        // watering check
        controller.All_FarmTile_WateringCheck();
        // health check
        controller.All_FarmTile_HealthCheck();
        // update sprite
        controller.All_FarmTile_Progress_Update();

        ReCalculate_AllSeed_WaitTime();
    }

    // real time system
    private void Save_MyTime()
    {
        ES3.Save("mySec", mySec);
    }
    private void Load_MyTime()
    {
        // load game
        if (ES3.KeyExists("mySec"))
        {
            mySec = ES3.Load<float>("mySec");
        }
    }

    private void Set_PastTime()
    {
        float hour = System.DateTime.Now.Hour * 3600f;
        float min = System.DateTime.Now.Minute * 60f;
        float sec = System.DateTime.Now.Second;

        pastSec = hour + min + sec;
        // save time
        ES3.Save("pastSec", pastSec);
    }
    private void Set_FutureTime()
    {
        float hour = System.DateTime.Now.Hour * 3600f;
        float min = System.DateTime.Now.Minute * 60f;
        float sec = System.DateTime.Now.Second;

        futureSec = hour + min + sec;
        // save time
        ES3.Save("futureSec", futureSec);
    }
    private void SubtractTime_SinceExit()
    {
        if (ES3.KeyExists("pastSec") && ES3.KeyExists("futureSec"))
        {
            float pastSec = ES3.Load<float>("pastSec");
            float futureSec = ES3.Load<float>("futureSec");
            float subtractTime;

            // if the game was opened and exit between pm and am time
            if (futureSec < pastSec)
            {
                subtractTime = (futureSec + 86400f) - pastSec;
            }
            else
            {
                subtractTime = futureSec - pastSec;
            }

            mySec -= subtractTime;
        }
    }

    private void Run_Time()
    {
        if (mySec > 0)
        {
            mySec -= Time.deltaTime; // make this work on exit run
        }
    }
    private void MyTime_Text_Update()
    {
        var x = controller.defaultMenu.menuUI.remainingTimeText;

        x.text = mySec.ToString("f0");
    }
    private void NextDay_Button_Availability()
    {
        if (mySec <= 0)
        {
            // set mySec to not ever go bellow 0
            mySec = 0;

            controller.defaultMenu.Activate_NextDay_Button(true);
        }
        else
        {
            controller.defaultMenu.Activate_NextDay_Button(false);
        }
    }
    private void ReCalculate_AllSeed_WaitTime()
    {
        var x = controller.farmTiles;

        for (int i = 0; i < x.Length; i++)
        {
            if (x[i].data.seedPlanted && !x[i].tileSeedStatus.harvestReady)
            {
                Add_MyTime(x[i].data.plantedSeed.waitTime);
            }
        }
    }

    public void Add_MyTime(float time)
    {
        mySec += time;
    }
    public void Subtract_MyTime(float time)
    {
        mySec -= time;
    }
}
