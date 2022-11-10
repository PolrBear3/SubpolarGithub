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
    public bool isFullScreen;
}

[System.Serializable]
public class Resolution_Controller_UI
{
    public Text currentResolution;
}

public class Resolution_Controller : MonoBehaviour
{
    public Resolution_Controller_Data data;
    public Resolution_Controller_UI ui;

    public Available_Resolution[] resolutions;

    private void Start()
    {
        Update_Current_Resolution_Text(0);
    }

    private void Update_Current_Resolution_Text(int arrayNum)
    {
        ui.currentResolution.text = resolutions[arrayNum].width + " * " + resolutions[arrayNum].height.ToString();
    }

    public void FullScreen_OnOff()
    {
        if (!data.isFullScreen)
        {
            data.isFullScreen = true;
            Screen.fullScreen = data.isFullScreen;
        }
        else
        {
            data.isFullScreen = false;
            Screen.fullScreen = data.isFullScreen;
        }
    }
    public void Change_Resolution(int arrayNum)
    {
        if (data.isFullScreen)
        {
            Screen.SetResolution(resolutions[arrayNum].width, resolutions[arrayNum].height, true);
        }
        else
        {
            Screen.SetResolution(resolutions[arrayNum].width, resolutions[arrayNum].height, false);
        }

        Update_Current_Resolution_Text(arrayNum);
    }
}
