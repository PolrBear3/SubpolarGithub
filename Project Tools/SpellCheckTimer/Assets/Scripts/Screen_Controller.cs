using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Resolutions
{
    public int width;
    public int height;
}

public class Screen_Controller : MonoBehaviour
{
    public Resolutions[] resolutions;
    public int currentID = 1;

    private void Start()
    {
        Set_Resolution();
    }

    public void Increase_Resolution()
    {
        currentID += 1;
        SetMaxMin_CurrentID();
        Set_Resolution();
    }
    public void Decrease_Resolution()
    {
        currentID -= 1;
        SetMaxMin_CurrentID();
        Set_Resolution();
    }

    private void SetMaxMin_CurrentID()
    {
        // max
        if (currentID > 2)
        {
            currentID = 2;
        }
        // min
        if (currentID < 0)
        {
            currentID = 0;
        }
    }
    private void Set_Resolution()
    {
        var x = resolutions[currentID];

        if (Screen.fullScreen)
        {
            Screen.SetResolution(x.width, x.height, true);
        }
        else
        {
            Screen.SetResolution(x.width, x.height, false);
        }
    }
}
