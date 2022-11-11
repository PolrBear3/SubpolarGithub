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
    public int currentWidth, currentHeight;
}

[System.Serializable]
public class Resolution_Controller_UI
{
    public Text currentResolution;
    public Text currentScreenMode;
}

public class Resolution_Controller : MonoBehaviour
{
    public Resolution_Controller_Data data;
    public Resolution_Controller_UI ui;

    public Available_Resolution[] resolutions;

    private void Start()
    {
        Change_Resolution(1);
    }

    private void Update_Current_Resolution_Text(int arrayNum)
    {
        ui.currentResolution.text = resolutions[arrayNum].width + " * " + resolutions[arrayNum].height.ToString();
    }

    public void Change_ScreenMode(bool on)
    {
        if (on == true)
        {
            data.isFullScreen = true;
            Screen.SetResolution(data.currentWidth, data.currentHeight, true);
            ui.currentScreenMode.text = "Full".ToString();
        }
        
        if (on == false)
        {
            data.isFullScreen = false;
            Screen.SetResolution(data.currentWidth, data.currentHeight, false);
            ui.currentScreenMode.text = "Window".ToString();
        }
    }
    public void Change_Resolution(int arrayNum)
    {
        if (data.isFullScreen)
        {
            Screen.SetResolution(resolutions[arrayNum].width, resolutions[arrayNum].height, true);
            data.currentWidth = resolutions[arrayNum].width;
            data.currentHeight = resolutions[arrayNum].height;
        }
        else
        {
            Screen.SetResolution(resolutions[arrayNum].width, resolutions[arrayNum].height, false);
            data.currentWidth = resolutions[arrayNum].width;
            data.currentHeight = resolutions[arrayNum].height;
        }

        Update_Current_Resolution_Text(arrayNum);
    }
}
