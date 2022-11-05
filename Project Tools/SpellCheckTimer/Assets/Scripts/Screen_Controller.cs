using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Screen_Controller : MonoBehaviour
{
    public void HD_Resolution()
    {
        Screen.SetResolution(720, 1280, false);
    }

    public void FullHD_Resolution()
    {
        Screen.SetResolution(1080, 1920, false);
    }
}
