using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

public class Resolution_Controller : MonoBehaviour
{
    public Resolution_Controller_Data data;
    public Available_Resolution[] resolutions;

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
    }
}
