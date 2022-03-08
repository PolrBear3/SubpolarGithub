using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EscapePod_ChairBed_Panel : MonoBehaviour
{
    public GameObject EscapePod_ChairBed_gameObject;
    public EscapePod_ChairBed_MainController controller;

    private void Update()
    {
        Icon_Popup();
    }

    public void Rotate()
    {

    }

    void Icon_Popup()
    {
        if (controller.playerDetection == true)
        {
            controller.icon.SetActive(true);
        }
        if (controller.playerDetection == false)
        {
            controller.icon.SetActive(false);
        }
    }
}
