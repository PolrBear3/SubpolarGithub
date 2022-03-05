using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EscapePod_ChairBed : MonoBehaviour
{
    public GameObject ChairBed_options;

    public GameObject positionButton1, positionButton2, positionButton3, positionButton4, positionButton5, positionButton6, positionButton7;

    public GameObject snapPoint1, snapPoint2, snapPoint3, snapPoint4, snapPoint5, snapPoint6, snapPoint7;

    private void Update()
    {
        snapCheck();
    }

    public void ChairBed_Options_On()
    {
        ChairBed_options.SetActive(true);
    }

    public void ChairBed_Options_Off()
    {
        ChairBed_options.SetActive(false);
    }

    private void snapCheck()
    {
        if (snapPoint1.activeSelf == false)
        {
            positionButton1.SetActive(false);
        }
        else if (snapPoint1.activeSelf == true)
        {
            positionButton1.SetActive(true);
        }

        if (snapPoint2.activeSelf == false)
        {
            positionButton2.SetActive(false);
        }
        else if (snapPoint2.activeSelf == true)
        {
            positionButton2.SetActive(true);
        }

        if (snapPoint3.activeSelf == false)
        {
            positionButton3.SetActive(false);
        }
        else if (snapPoint3.activeSelf == true)
        {
            positionButton3.SetActive(true);
        }

        if (snapPoint4.activeSelf == false)
        {
            positionButton4.SetActive(false);
        }
        else if (snapPoint4.activeSelf == true)
        {
            positionButton4.SetActive(true);
        }

        if (snapPoint5.activeSelf == false)
        {
            positionButton5.SetActive(false);
        }
        else if (snapPoint5.activeSelf == true)
        {
            positionButton5.SetActive(true);
        }

        if (snapPoint6.activeSelf == false)
        {
            positionButton6.SetActive(false);
        }
        else if (snapPoint6.activeSelf == true)
        {
            positionButton6.SetActive(true);
        }

        if (snapPoint7.activeSelf == false)
        {
            positionButton7.SetActive(false);
        }
        else if (snapPoint7.activeSelf == true)
        {
            positionButton7.SetActive(true);
        }
    }
}
