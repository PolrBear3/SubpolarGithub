using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gadget_MainController : MonoBehaviour
{
    public static bool isGadgetMenuOn = false;

    // gadget menus
    public GameObject[] menus;

    public void TurnOff_All_Menu()
    {
        for (int i=0; i < menus.Length; i++)
        {
            if (menus[i].activeSelf)
            {
                menus[i].SetActive(false);
                break;
            }
        }
    }
}
