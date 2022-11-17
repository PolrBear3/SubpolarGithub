using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public struct Available_Resolution
{
    public int width;
    public int height;
}

[System.Serializable]
public class Resolution_Controller_Data
{
    public bool isFullScreen = false;
    public int currentResArrayNum;

    [HideInInspector]
    public int preResArrayNum;
    [HideInInspector]
    public bool isPreFullScreen = false;
    [HideInInspector]
    public bool countApplyResTime = false;
    [HideInInspector]
    public float currentApplyResTime;
    public float maxApplyResTime;
}

[System.Serializable]
public class Resolution_Controller_UI
{
    public Text currentResolution;
    public Text currentScreenMode;

    public GameObject appyResPanel;
    public Text applyResTimeText;
}

public class Resolution_Controller : MonoBehaviour
{
    public Resolution_Controller_Data data;
    public Resolution_Controller_UI ui;

    public Available_Resolution[] resolutions;

    private void Start()
    {
        Load_Resolution_and_ScreenMode();
    }
    private void Update()
    {
        Resolution_Panel_Timer();
    }

    private void Update_Current_Resolution_Text(int arrayNum)
    {
        ui.currentResolution.text = resolutions[arrayNum].width + " * " + resolutions[arrayNum].height.ToString();
    }
    private void Update_Current_ScreenMode_Text(bool isFullScreen)
    {
        if (isFullScreen)
        {
            ui.currentScreenMode.text = "Full".ToString(); ;
        }
        else
        {
            ui.currentScreenMode.text = "Window".ToString(); ;
        }
    }

    private void Load_Resolution_and_ScreenMode()
    {
        // default resolution for test (function not complete)
        data.currentResArrayNum = 2;
        var x = data.currentResArrayNum;
        Screen.SetResolution(resolutions[x].width, resolutions[x].height, data.isFullScreen);

        Update_Current_Resolution_Text(x);
    }
    
    public void Change_ScreenMode(bool on)
    {
        if (on != data.isFullScreen)
        {
            Resolution_Panel_Popup();

            var x = data.currentResArrayNum;
            if (on == true)
            {
                data.isFullScreen = true;
                Screen.SetResolution(resolutions[x].width, resolutions[x].height, true);
            }
            else
            {
                data.isFullScreen = false;
                Screen.SetResolution(resolutions[x].width, resolutions[x].height, false);
            }

            Update_Current_ScreenMode_Text(on);
        }
    }
    public void Change_Resolution(int arrayNum)
    {
        if (arrayNum != data.currentResArrayNum)
        {
            Resolution_Panel_Popup();

            if (data.isFullScreen)
            {
                Screen.SetResolution(resolutions[arrayNum].width, resolutions[arrayNum].height, true);
            }
            else
            {
                Screen.SetResolution(resolutions[arrayNum].width, resolutions[arrayNum].height, false);
            }
            data.currentResArrayNum = arrayNum;

            Update_Current_Resolution_Text(arrayNum);
        }
    }

    private void Resolution_Panel_Timer()
    {
        // if apply res panel is on, reset timer and start the timer
        if (data.countApplyResTime)
        {
            // time text update
            ui.applyResTimeText.text = data.currentApplyResTime.ToString("f0");
            
            // count delta time
            data.currentApplyResTime -= Time.deltaTime;
            // if timer reaches 0, Reset_Back_Resolution()
            if (data.currentApplyResTime <= 0)
            {
                data.countApplyResTime = false;
                Reset_Back_Resolution();
            }
        }
    }
    private void Resolution_Panel_Popup()
    {
        // save pre apply resolution
        data.preResArrayNum = data.currentResArrayNum;
        data.isPreFullScreen = data.isFullScreen;
        // apply res panel shows up
        ui.appyResPanel.SetActive(true);
        // set timer
        data.currentApplyResTime = data.maxApplyResTime;
        // start timer
        data.countApplyResTime = true;
        // all the buttons on the back, unfunctional while it is showing
    }
    
    public void Apply_Resolution()
    {
        // deactivate apply res panel
        ui.appyResPanel.SetActive(false);
        // stop timer
        data.countApplyResTime = false;
        // all the buttons on the back, functional
    }
    public void Reset_Back_Resolution()
    {
        // deactivate apply res panel
        ui.appyResPanel.SetActive(false);
        // stop timer
        data.countApplyResTime = false;
        // set back to the past resolution, update data and ui
        var x = data.preResArrayNum;
        Screen.SetResolution(resolutions[x].width, resolutions[x].height, data.isPreFullScreen);
        data.currentResArrayNum = x;
        data.isFullScreen = data.isPreFullScreen;
        Update_Current_Resolution_Text(x);
        Update_Current_ScreenMode_Text(data.isFullScreen);
        // all the buttons on the back, functional
    }
}
