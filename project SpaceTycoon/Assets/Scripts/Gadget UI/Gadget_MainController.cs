using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gadget_MainController : MonoBehaviour
{
    public static bool isGadgetMenuOn = false;

    // gadget menus
    public List<GameObject> gadgetMenus;

    public void TurnOff_All_Menu()
    {
        isGadgetMenuOn = false;

        foreach(var menu in gadgetMenus)
        {
            menu.SetActive(false);
        }
    }
}
